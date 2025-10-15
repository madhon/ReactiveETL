using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;

namespace ReactiveETL.Tests
{
    /// <summary>
    /// Accès à un fichier de ressource
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Lit un objet depuis un fichier xml dans lequel l'objet a été sérialisé
        /// </summary>
        /// <typeparam name="T">Type de l'objet</typeparam>
        /// <param name="ressourceFileName">nom du fichier de ressources</param>
        /// <returns>objet désérialisé</returns>
        public static T ReadObject<T>(string ressourceFileName)
        {
            T res = default(T);
            string content = Helper.LoadFromRessourceFile(ressourceFileName);
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(content))
            {
                res = (T)serializer.Deserialize(reader);
            }

            return res;
        }

        /// <summary>
        /// Load a ressource file into a string
        /// </summary>
        /// <param name="ressourceFileName">chemin d'accès</param>
        /// <returns>Fichier lu</returns>
        public static string LoadFromRessourceFile(string ressourceFileName)
        {
            string res = null;
            UseRessourceStream(
                ressourceFileName,
                input =>
                {
                    using (var reader = new StreamReader(input))
                    {
                        res = reader.ReadToEnd();
                    }
                });

            return res;
        }

        /// <summary>
        /// Load a ressource file into a string
        /// </summary>
        /// <param name="ressourceFileName">chemin d'accès</param>
        /// <returns>stream du fichier</returns>
        public static StreamReader LoadFromRessourceFileToStreamReader(string ressourceFileName)
        {
            Stream file3 = null;
            Assembly ass = typeof(Helper).Assembly;
            string[] ressources = ass.GetManifestResourceNames();
            foreach (string resname in ressources)
            {
                if (resname.EndsWith(ressourceFileName, StringComparison.OrdinalIgnoreCase))
                {
                    file3 = ass.GetManifestResourceStream(resname);
                }
            }

            return new StreamReader(file3, Encoding.Default);
        }

        /// <summary>
        /// Load a ressource file into a string
        /// </summary>
        /// <param name="ressourceFileName">chemin d'accès</param>
        /// <returns>stream du fichier</returns>
        public static Stream LoadFromRessourceFileToStream(string ressourceFileName)
        {
            Stream file3 = null;
            Assembly ass = typeof(Helper).Assembly;
            string[] ressources = ass.GetManifestResourceNames();
            foreach (string resname in ressources)
            {
                if (resname.EndsWith(ressourceFileName, StringComparison.OrdinalIgnoreCase))
                {
                    file3 = ass.GetManifestResourceStream(resname);
                }
            }

            return file3;
        }

        /// <summary>
        /// Load a ressource file
        /// </summary>
        /// <param name="ressourceFileName">
        /// chemin d'accès
        /// </param>
        /// <param name="actInput">
        /// The act input.
        /// </param>
        public static void UseRessourceStream(string ressourceFileName, Action<Stream> actInput)
        {
            Assembly ass = typeof(Helper).Assembly;
            string[] ressources = ass.GetManifestResourceNames();
            foreach (string resname in ressources)
            {
                if (resname.EndsWith(ressourceFileName, StringComparison.OrdinalIgnoreCase))
                {
                    using (Stream input = ass.GetManifestResourceStream(resname))
                    {
                        actInput(input);
                    }
                }
            }
        }
    }
}
