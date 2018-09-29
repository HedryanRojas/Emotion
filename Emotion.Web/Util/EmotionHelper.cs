using Emotion.Models;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Emotion.Web.Util
{
    public class EmotionHelper
    {
        public EmotionServiceClient emoClient;
        public EmotionHelper(string key)
        {
            emoClient = new EmotionServiceClient(key);
        }

        public  async Task<EmoPicture> DetectAndExtractFacesAsync(Stream imageStream)
        {
            Microsoft.ProjectOxford.Emotion.Contract.Emotion[] emotions = await emoClient.RecognizeAsync(imageStream);
            var emoPicture = new EmoPicture();

            emoPicture.Faces = ExtractFaces(emotions,emoPicture);

            return emoPicture;
        }

        private ObservableCollection<EmoFace> ExtractFaces(Microsoft.ProjectOxford.Emotion.Contract.Emotion[] emotions, EmoPicture emoPicture)
        {
            var  facesList = new ObservableCollection<EmoFace>();
            foreach (var emotion in emotions)
            {
                var emoFace = new EmoFace()
                {
                    X = emotion.FaceRectangle.Left,
                    Y = emotion.FaceRectangle.Top,
                    Width = emotion.FaceRectangle.Width,
                    Height = emotion.FaceRectangle.Height,
                    Picture = emoPicture,
                };
                emoFace.Emotions = ProcessEmotions(emotion.Scores, emoFace);
                facesList.Add(emoFace);
            }
            return facesList;
        }

        private ObservableCollection<EmoEmotion> ProcessEmotions(EmotionScores scores, EmoFace emoFace)
        {
            var emotionList = new ObservableCollection<EmoEmotion>();
            var properties = scores.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var filterProperties = properties.Where(p => p.PropertyType == typeof(float));
            var emoType = EmoEmotionEnum.Undertermined;
            foreach (var prop in filterProperties)
            {
                if(!Enum.TryParse<EmoEmotionEnum>(prop.Name, out emoType))
                    emoType=EmoEmotionEnum.Undertermined;
                var emoEmotion = new EmoEmotion();
                emoEmotion.Score = (float) prop.GetValue(scores);
                emoEmotion.EmotionType = emoType;
                emoEmotion.Face = emoFace;
                emotionList.Add(emoEmotion);
            }
            return emotionList;
        }
    }
}