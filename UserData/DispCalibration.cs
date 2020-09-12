using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using BaseDll;
namespace UserData
{
    public class DispCalibration
    {
        public double Row { get; set; }
        public double Col { get; set; }
        public double MotionX { get; set; }
        public double MotionY{ get; set; }
    }
    public class DispCalibrationFile
    {
        public static bool Read(int index, string path)
        {
            UserTest.DispCalibrationAB[index] = (List<DispCalibration>)AccessXmlSerializer.XmlToObject(path, typeof(List<DispCalibration>));
            if (UserTest.DispCalibrationAB[index] == null)
            {
                UserTest.DispCalibrationAB[index] = new List<DispCalibration>();
                return false;
            }
            return true;
        }
        public static void Save(int index, string path)
        {
            AccessXmlSerializer.ObjectToXml(path, UserTest.DispCalibrationAB[index]);
        }
    }





}


