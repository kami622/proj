namespace EduInfoSys.Helpers
{
	public class FileHelper
	{
		public static (bool, string) Check(IFormFile file)
		{
			bool check = true;
			string msg = string.Empty;
			if (file.ContentType != "text/plain" && file.ContentType != "application/msword" && 
				file.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document" && 
				file.ContentType != "image/png" && file.ContentType != "image/jpeg" && file.ContentType != "application/vnd.ms-powerpoint" &&
				file.ContentType != "application/vnd.openxmlformats-officedocument.presentationml.presentation" &&
				file.ContentType != "application/vnd.ms-excel" && file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" &&
				file.ContentType != "application/pdf" && file.ContentType != "application/x-zip-compressed")
			{
				check = false;
				msg = "File type is invalid";
			}
			else if (file.Length > 10485760)
			{
				check = false;
				msg = "File size must be less than 10 MB";
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
