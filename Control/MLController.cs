using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using ML.Model;

namespace ML.Control
{
    public class MLController
    {
        public string _assetsPath { get; set; }
        public string _imagesFolder => Path.Combine(_assetsPath, "cars_asset");
        public string _modelFolder => Path.Combine(_assetsPath, "model");
        private string _trainTagsTsv => Path.Combine(_imagesFolder, "tags.tsv");
        private string _testTagsTsv => Path.Combine(_imagesFolder, "test-tags.tsv");

        private string _testFolder => Path.Combine(_assetsPath, "test_images");

        private string _inceptionTensorFlowModel => Path.Combine(_assetsPath, "inception", "tensorflow_inception_graph.pb");

        private MLContext _mlContext;
        private ITransformer _model;
        private IDataView _data;

        private static MLController _instance;

        private MLController(string result)
        {
            _assetsPath = result;
            _mlContext = new MLContext();
        }

        public static MLController GetInstance(string result = "")
        {
            if (_instance == null)
            {
                _instance = new MLController(result);
            }

            return _instance;
        }

        public void InitializeAndLoadData()
        {
            _model = GenerateModel(_mlContext);
            TestImages();
        }

        private void TestImages()
        {
            var images = new DirectoryInfo(_testFolder).GetFiles();

            foreach (var image in images)
            {
                ClassifySingleTestImage(image.FullName, Path.GetFileNameWithoutExtension(image.Name));
            }
        }

        private void ClassifySingleTestImage(string path, string expectedResult)
        {
            var imageData = new ImageData()
            {
                ImagePath = path
            };
            
            var predictor = _mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(_model);
            var prediction = predictor.Predict(imageData);

            if (expectedResult.Contains(prediction.PredictedLabelValue))
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.WriteLine($"Image: {Path.GetFileName(imageData.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} Expected: {Path.GetFileNameWithoutExtension(path)}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        #region NoTouchRegion

        private void DisplayResults(IEnumerable<ImagePrediction> imagePredictionData)
        {
            foreach (ImagePrediction prediction in imagePredictionData)
            {
                Console.WriteLine($"Image: {Path.GetFileName(prediction.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
            }
        }

        public ImagePrediction ClassifySingleImage(string path)
        {
            var imageData = new ImageData()
            {
                ImagePath = path
            };

            var predictor = _mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(_model);
            var prediction = predictor.Predict(imageData);
            Console.WriteLine($"Image: {Path.GetFileName(imageData.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
            return prediction;
        }

        private ITransformer GenerateModel(MLContext mlContext)
        {
            _data = mlContext.Data.LoadFromTextFile<ImageData>(path: _trainTagsTsv, hasHeader: false);

            IEstimator<ITransformer> pipeline = mlContext.Transforms.LoadImages(outputColumnName: "input", imageFolder: _imagesFolder, inputColumnName: nameof(ImageData.ImagePath))
                // The image transforms transform the images into the model's expected format.
                .Append(mlContext.Transforms.ResizeImages(outputColumnName: "input", imageWidth: InceptionSettings.ImageWidth, imageHeight: InceptionSettings.ImageHeight, inputColumnName: "input"))
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input", interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean))
                .Append(mlContext.Model.LoadTensorFlowModel(_inceptionTensorFlowModel).
                    ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: "Label"))
                .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: "softmax2_pre_activation"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
                .AppendCacheCheckpoint(mlContext);

            ITransformer model = pipeline.Fit(_data);

            IDataView testData = mlContext.Data.LoadFromTextFile<ImageData>(path: _testTagsTsv, hasHeader: false);
            IDataView predictions = model.Transform(testData);

            // Create an IEnumerable for the predictions for displaying results
            IEnumerable<ImagePrediction> imagePredictionData = mlContext.Data.CreateEnumerable<ImagePrediction>(predictions, true);
            DisplayResults(imagePredictionData);

            MulticlassClassificationMetrics metrics =
                mlContext.MulticlassClassification.Evaluate(predictions,
                    labelColumnName: "LabelKey", predictedLabelColumnName: "PredictedLabel");

            Console.WriteLine($"LogLoss is: {metrics.LogLoss}");
            Console.WriteLine($"PerClassLogLoss is: {String.Join(" , ", metrics.PerClassLogLoss.Select(c => c.ToString()))}");


            return model;
        }

        struct InceptionSettings
        {
            public const int ImageHeight = 224;
            public const int ImageWidth = 224;
            public const float Mean = 117;
            public const float Scale = 1;
            public const bool ChannelsLast = true;
        }

        #endregion

        public void LoadModel()
        {
            _model = _mlContext.Model.Load(Path.Combine(_modelFolder, "model.zip"), out var modelSchema);
            TestImages();
        }

        public void SaveModel()
        {
            _mlContext.Model.Save(_model, _data.Schema, Path.Combine(_modelFolder, "model.zip"));
        }
    }
}
