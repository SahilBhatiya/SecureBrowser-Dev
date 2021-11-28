using System;

namespace SecureBrowser.OtherFunctions.Models
{
    public class StudentCopied
    {
        public string Id { get; set; }
        public string crrTime { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }

        public StudentCopied()
        {
            Id = Guid.NewGuid().ToString();
            crrTime = DateTime.Now.ToString();
        }
    }
}
