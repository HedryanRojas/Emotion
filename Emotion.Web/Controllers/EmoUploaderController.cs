using Emotion.Web.Models;
using Emotion.Web.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Emotion.Web.Controllers
{
    public class EmoUploaderController : Controller
    {
        string serverFolderPath;
        EmotionHelper emotionHelper;
        string key;
        EmotionWebContext db = new EmotionWebContext();

        public EmoUploaderController()
        {
            serverFolderPath = ConfigurationManager.AppSettings["UPLOAD_DIR"];
            key = ConfigurationManager.AppSettings["EMOTION_KEY"];
            emotionHelper = new EmotionHelper(key);
        }
        // GET: EmoUploader
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Index(HttpPostedFileBase file)
        {
            var pictureName = Guid.NewGuid().ToString();
            pictureName += Path.GetExtension(file.FileName);

            var route = Server.MapPath(serverFolderPath);

            route = route + "/" + pictureName;
            if (file?.ContentLength > 0)
            {
                file.SaveAs(route);
                var emoPicture = await emotionHelper.DetectAndExtractFacesAsync(file.InputStream);
                emoPicture.Name = file.FileName;
                emoPicture.Path = $"{serverFolderPath}/{pictureName}";
                db.EmoPictures.Add(emoPicture);
                await db.SaveChangesAsync();
                RedirectToAction("Details", "EmoPicture", new { Id = emoPicture.Id });
            }
            return View();
        }
    }
}