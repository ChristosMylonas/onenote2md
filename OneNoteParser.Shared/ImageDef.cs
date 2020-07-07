using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneNoteParser.Shared
{
    public class ImageDef
    {
        bool withinImage;
        decimal width;
        decimal height;
        string format;
        int count;
        


        public ImageDef()
        {
            withinImage = false;
        }

        public void SetWithinImage(string format)
        {
            withinImage = true;
            this.format = format;
            count++;
        }

        public void SetDimensions(decimal width, decimal height)
        {
            this.width = width;
            this.height = height;
        }


        public void Reset()
        {
            withinImage = false;
        }

        public bool IsWithinImage()
        {
            return withinImage;
        }

        public string GetFilename(string baseFilename)
        {
            return $"{baseFilename}_{count}.{format}";
        }


    }
}
