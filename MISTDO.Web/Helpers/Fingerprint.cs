using DPFP;
using DPFP.Capture;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace MISTDO.Web.Helpers
{
    public class Fingerprint : DPFP.Capture.EventHandler
    {
        public Capture capture { get; set; } = new Capture();
        DPFP.Sample sample = new DPFP.Sample();

        public void EnrollAndSavePicture()
        {
            capture.EventHandler = this;
            capture.StartCapture();
        }

        public void OnComplete(object capture, string readerSerialNumber, Sample sample)
        {
            ((Capture)capture).StopCapture();

            var sampleConvertor = new SampleConversion();
           

           
            Bitmap bitmap = null;
            byte[] image = sample.Bytes;

           
            bitmap.Save(@"C:\Users\jeremy\Desktop\fingerprint.bmp");
        }

        public void OnFingerGone(object capture, string readerSerialNumber) { }
        public void OnFingerTouch(object capture, string readerSerialNumber) { }
        public void OnReaderConnect(object capture, string readerSerialNumber) { }
        public void OnReaderDisconnect(object capture, string readerSerialNumber) { }
        public void OnSampleQuality(object capture, string readerSerialNumber, CaptureFeedback captureFeedback) { }
    }
}
