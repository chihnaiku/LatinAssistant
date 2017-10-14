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
        First = 0,
        Second= 1,
        Third = 2,

    }
    public enum NumberTypes
    {
        Singular = 0,
        Plural = 1

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
        public NumberTypes NumberType
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
                return String.Format(
@"
VerbumType:{0}
PersonType:{1}
NumberType:{6}
TenseType:{2}
MoodType:{3}
VoiceType:{4}
DictionaryTerm:{5}", VerbumType, PersonType, TenseType, MoodType, VoiceType, DictionaryTerm, NumberType);
            }
            else if (VerbType == VerbTypes.Infinitive)
            {
                return String.Format(
@"
VerbumType: Infinitive
TenseType:{0}
VoiceType:{1}
BaseForm:{2}", TenseType, VoiceType, BaseForm);
            }
            else if (VerbType == VerbTypes.Conjugation)
            {
                return String.Format(
@"
VerbumType:{0}
PersonType:{1}
NumberType:{6}
TenseType:{2}
MoodType:{3}
VoiceType:{4}
BaseForm:{5}", VerbumType, PersonType, TenseType, MoodType, VoiceType, BaseForm, NumberType);
            }
            else
            {
                return base.ToString();
            }
        }
    }
    public class Participle : Verbum
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
        static List<Verbum> QueryVerba(string word)
        {
            List<Verbum> verbumList = new List<Verbum>();
            HtmlAgilityPack.HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load("https://en.wiktionary.org/wiki/" + word);
            if (doc.DocumentNode.SelectNodes("//div[@class='mw-parser-output']//h2") != null)
            {
                string languageName = "";
                foreach (HtmlNode languageNameNode in doc.DocumentNode.SelectNodes("//div[@class='mw-parser-output']/h2"))
                {
                    languageName = GetTitle(languageNameNode);

                    if (languageName == "Latin")
                    {
                        HtmlNode nextNode = languageNameNode.NextSibling;
                        while (nextNode != null)
                        {
                            if (nextNode.OriginalName != "h2")
                            {
                                if (nextNode.OuterHtml.Length > 1)
                                {
                                    if (nextNode.OriginalName.StartsWith("h"))
                                    {
                                        string sectionName = GetTitle(nextNode);
                                        if (sectionName == "Verb")
                                        {
                                            HtmlNode pNode = nextNode.NextSibling.NextSibling;
                                            HtmlNode olNode = pNode.NextSibling.NextSibling;

                                            //Console.WriteLine("Verb Found");
                                            if (pNode.ChildNodes.Count > 5)
                                            {
                                                Verb newVerb = new Verb();
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
                                                newVerb.PersonType = PersonTypes.First;
                                                newVerb.NumberType = NumberTypes.Singular;
                                                newVerb.TenseType = TenseTypes.Present;
                                                newVerb.VoiceType = VoiceTypes.Active;
                                                verbumList.Add(newVerb);
                                            }
                                            else
                                            {

                                                foreach (HtmlNode liNode in olNode.SelectNodes("./li"))
                                                {
                                                    Verb newVerb = new Verb();
                                                    newVerb.BaseForm = liNode.SelectSingleNode(".//i/a").InnerText;

                                                    if (liNode.InnerHtml.Contains("infinitive"))
                                                    {
                                                        newVerb.VerbType = VerbTypes.Infinitive;
                                                        switch(liNode.SelectSingleNode("./span/a[1]").InnerText)
                                                        {
                                                            case "present":
                                                                newVerb.TenseType = TenseTypes.Present;
                                                                break;
                                                            case "perfect":
                                                                newVerb.TenseType = TenseTypes.Perfect;
                                                                break;
                                                        }
                                                        switch (liNode.SelectSingleNode("./span/a[2]").InnerText)
                                                        {
                                                            case "active":
                                                                newVerb.VoiceType = VoiceTypes.Active;
                                                                break;
                                                            case "passive":
                                                                newVerb.VoiceType = VoiceTypes.Passive;
                                                                break;
                                                        }
                                                        verbumList.Add(newVerb);
                                                    }
                                                    else
                                                    {
                                                        newVerb.VerbType = VerbTypes.Conjugation;
                                                        switch (liNode.SelectSingleNode("./span/a[1]").InnerText)
                                                        {
                                                            case "first-person":
                                                                newVerb.PersonType = PersonTypes.First;
                                                                break;
                                                            case "second-person":
                                                                newVerb.PersonType = PersonTypes.Second;
                                                                break;
                                                            case "third-person":
                                                                newVerb.PersonType = PersonTypes.Third;
                                                                break;
                                                        }
                                                        switch (liNode.SelectSingleNode("./span/a[2]").InnerText)
                                                        {
                                                            case "singular":
                                                                newVerb.NumberType = NumberTypes.Singular;
                                                                break;
                                                            case "plural":
                                                                newVerb.NumberType = NumberTypes.Plural;
                                                                break;
                                                        }
                                                        if (liNode.SelectNodes("./span/a").Count == 5)
                                                        {
                                                            switch (liNode.SelectSingleNode("./span/a[3]").InnerText)
                                                            {
                                                                case "present":
                                                                    newVerb.TenseType = TenseTypes.Present;
                                                                    break;
                                                                case "perfect":
                                                                    newVerb.TenseType = TenseTypes.Perfect;
                                                                    break;
                                                                case "imperfect":
                                                                    newVerb.TenseType = TenseTypes.Imperfect;
                                                                    break;
                                                                case "future":
                                                                    newVerb.TenseType = TenseTypes.Future;
                                                                    break;
                                                                case "pluperfect":
                                                                    newVerb.TenseType = TenseTypes.Pluperfect;
                                                                    break;
                                                            }
                                                            switch (liNode.SelectSingleNode("./span/a[4]").InnerText)
                                                            {
                                                                case "active":
                                                                    newVerb.VoiceType = VoiceTypes.Active;
                                                                    break;
                                                                case "passive":
                                                                    newVerb.VoiceType = VoiceTypes.Passive;
                                                                    break;
                                                            }
                                                            switch (liNode.SelectSingleNode("./span/a[5]").InnerText)
                                                            {
                                                                case "indicative":
                                                                    newVerb.MoodType = MoodTypes.Indicative;
                                                                    break;
                                                                case "imperative":
                                                                    newVerb.MoodType = MoodTypes.Imperative;
                                                                    break;
                                                                case "subjunctive":
                                                                    newVerb.MoodType = MoodTypes.Subjunctive;
                                                                    break;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            newVerb.TenseType = TenseTypes.FuturePerfect;
                                                            switch (liNode.SelectSingleNode("./span/a[5]").InnerText)
                                                            {
                                                                case "active":
                                                                    newVerb.VoiceType = VoiceTypes.Active;
                                                                    break;
                                                                case "passive":
                                                                    newVerb.VoiceType = VoiceTypes.Passive;
                                                                    break;
                                                            }
                                                            switch (liNode.SelectSingleNode("./span/a[6]").InnerText)
                                                            {
                                                                case "indicative":
                                                                    newVerb.MoodType = MoodTypes.Indicative;
                                                                    break;
                                                                case "imperative":
                                                                    newVerb.MoodType = MoodTypes.Imperative;
                                                                    break;
                                                                case "subjunctive":
                                                                    newVerb.MoodType = MoodTypes.Subjunctive;
                                                                    break;
                                                            }
                                                        }
                                                        verbumList.Add(newVerb);
                                                    }

                                                }
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


                    }
                }
            }
            return verbumList;
        }
        static void Main(string[] args)
        {
            //string queryString = "amo moneo ago audio capio fio";
            string queryString = "laudo laudare laudaverint laudavisti";
            Console.WriteLine("The Query String is ");
            Console.WriteLine(queryString);
            Console.WriteLine("####Start Querying####");
            List<Verbum> verbumList = new List<Verbum>();
            foreach (string word in queryString.Split(' '))
            {
                verbumList = QueryVerba(word);
                Console.WriteLine(word);
                if (verbumList.Count >= 1)
                {
                    foreach (Verb v in verbumList)
                    {
                        Console.WriteLine(v.ToString());
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("This is not a latin word");
                }

            }

            Console.ReadKey();
        }
    }
}
