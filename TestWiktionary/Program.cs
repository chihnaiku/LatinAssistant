using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Web;
using HtmlAgilityPack;

namespace TestWiktionary
{
    public enum VerbTypes
    {
        FirstPersonSingular = 0,
        Conjugation = 1,
        Infinitive = 2
    }
    public enum MoodTypes
    {
        Indicative = 0,
        Subjunctive = 1,
        Imperative = 2
    }
    public enum VoiceTypes
    {
        Active = 0,
        Passive = 1
    }
    public enum TenseTypes
    {
        Present = 0,
        Imperfect = 1,
        Future = 2,
        Perfect = 3,
        Pluperfect = 4,
        FuturePerfect = 5
    }
    public enum PersonTypes
    {
        FirstSingular = 0,
        SecondSingular = 1,
        ThirdSingular = 2,
        FirstPlural = 3,
        SecondPlural = 4,
        ThirdPlural = 5
    }
    public enum VerbumTypes
    {
        Verb = 0,
        Participle = 1,
        Adverb = 3,
        Adjective = 4,
        Interjection = 5,
        Pronoun = 6,
        Preposition = 7
    }
    public class Verbum
    {
        public VerbumTypes VerbumType
        {
            get;
            set;
        }
    }
    public class Verb : Verbum
    {
        public Verb()
        {
            VerbumType = VerbumTypes.Verb;
        }
        public MoodTypes MoodType
        {
            get;
            set;
        }
        public TenseTypes TenseType
        {
            get;
            set;
        }
        public PersonTypes PersonType
        {
            get;
            set;
        }
        public VerbTypes VerbType
        {
            get;
            set;
        }
        public VoiceTypes VoiceType
        {
            get;
            set;
        }
        public string DictionaryTerm
        {
            get;
            set;
        }
        public string BaseForm
        {
            get;
            set;
        }
        public string Conjugation
        {
            get;
            set;
        }
        public override string ToString()
        {
            if (VerbType == VerbTypes.FirstPersonSingular)
            {
                return String.Format(@"PersonType:{0}
TenseType:{1}
MoodType:{2}
VoiceType:{3}
DictionaryTerm:{4}", PersonType, TenseType, MoodType, VoiceType, DictionaryTerm);
            }
            else
            {
                return base.ToString();
            }
        }
    }
    public class Participle:Verbum
    {
        public Participle()
        {
            VerbumType = VerbumTypes.Participle;
        }
        public string Case
        {
            get;
            set;
        }
        public string Gender
        {
            get;
            set;
        }
        public string Number
        {
            get;
            set;
        }
        public string BaseForm
        {
            get;
            set;
        }
    }
    class Program
    {
        static string GetTitle(HtmlNode node)
        {
            string result;
            if (node.SelectNodes("./span[@class='mw-headline']") != null)
            {
                result = node.SelectSingleNode("./span[@class='mw-headline']").InnerText;
            }
            else
            {
                result = null;
            }
            return result;
        }

        static void Main(string[] args)
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load("https://en.wiktionary.org/wiki/audeo");
            Boolean ifLatinExists = false;
            if (doc.DocumentNode.SelectNodes("//div[@class='mw-parser-output']//h2") != null)
            {
                string languageName = "";
                foreach (HtmlNode languageNameNode in doc.DocumentNode.SelectNodes("//div[@class='mw-parser-output']/h2"))
                {
                    languageName = GetTitle(languageNameNode);
                    Console.WriteLine(languageName);
                    if (languageName == "Latin")
                    {
                        ifLatinExists = true;
                        List<Verb> verbList = new List<Verb>();
                        HtmlNode nextNode = languageNameNode.NextSibling;
                        while (nextNode != null)
                        {
                            if (nextNode.OriginalName != "h2")
                            {
                                //Console.WriteLine(nextNode.OuterHtml.Length);
                                if (nextNode.OuterHtml.Length > 1)
                                {
                                    if (nextNode.OriginalName == "h3")
                                    {
                                        string sectionName = GetTitle(nextNode);
                                        if (sectionName == "Verb")
                                        {
                                            HtmlNode pNode = nextNode.NextSibling.NextSibling;
                                            HtmlNode olNode = pNode.NextSibling.NextSibling;
                                            Verb newVerb = new Verb();
                                            Console.WriteLine("Verb Found");
                                            if (pNode.ChildNodes.Count > 5)
                                            {
                                                newVerb.VerbType = VerbTypes.FirstPersonSingular;
                                                string dictionaryTermString = pNode.SelectSingleNode("./strong[@class='Latn headword']").InnerText;
                                                newVerb.BaseForm = dictionaryTermString;

                                                foreach (HtmlNode nodeInsideP in pNode.SelectNodes("./b/a"))
                                                {
                                                    dictionaryTermString += ",";
                                                    dictionaryTermString += nodeInsideP.InnerText;
                                                }
                                                //Console.WriteLine(dictionaryTermString);
                                                newVerb.DictionaryTerm = dictionaryTermString;
                                                newVerb.MoodType = MoodTypes.Indicative;
                                                newVerb.PersonType = PersonTypes.FirstSingular;
                                                newVerb.TenseType = TenseTypes.Present;
                                                newVerb.VoiceType = VoiceTypes.Active;
                                                verbList.Add(newVerb);
                                            }
                                            else
                                            {
                                                newVerb.VerbType = VerbTypes.Conjugation;
                                            }


                                        }

                                    }
                                }
                                nextNode = nextNode.NextSibling;
                            }
                            else
                            {
                                break;
                            }


                        }
                        foreach (Verb v in verbList)
                        {
                            Console.WriteLine(v.ToString());
                        }

                    }
                }
            }
            if (!ifLatinExists)
            {
                Console.WriteLine("This is not a latin word");
            }
            Console.ReadKey();
        }
    }
}
