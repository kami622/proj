namespace EduInfoSys.Helpers
{
	public static class ImageHelper
	{
		public static (bool, string) Check(IFormFile file)
		{
			bool check = true;
			string msg = string.Empty;
			if (file.ContentType != "image/png" && file.ContentType != "image/jpeg")
			{
				check = false;
				msg = "Image type must be PNG or JPG/JPEG";
			}
			else if (file.Length > 5242880)
			{
				check = false;
				msg = "Image size must be less than 5 MB";
			}
			return (check, msg);
		}

		public static string Save(string webRootPath, string folderPath, IFormFile file)
		{
			string filePath = file.FileName;
			if (filePath.Length > 64) filePath = filePath.Substring(filePath.Length - 64, 64);
			filePath = Guid.NewGuid().ToString() + filePath;
			string path = Path.Combine(webRootPath, folderPath, filePath);
			using (FileStream fs = new FileStream(path, FileMode.Create))
			{
				file.CopyTo(fs);
			}
			return filePath;
		}

		public static void Delete(string webRootPath, string folderPath, string filePath)
		{
			string path = Path.Combine(webRootPath, folderPath, filePath);
			if (File.Exists(path)) File.Delete(path);
		}
	}
}
