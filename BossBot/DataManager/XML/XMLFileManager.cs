using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DataManager.XML
{
    public static class XMLFileManager
    {
        public static T ReadFile<T>(string path) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T elements = null;
            try
            {
                using (StreamReader streamReader = new StreamReader(path))
                {
                    try
                    {
                        elements = (T)serializer.Deserialize(streamReader);
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        streamReader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return elements;
        }
        public static void WriteFile<T>(string path, T entity) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(path))
                {
                    try
                    {
                        serializer.Serialize(streamWriter, entity);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        streamWriter.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
