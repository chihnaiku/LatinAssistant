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
        Second = 1,
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
        Noun = 2,
        Adverb = 3,
        Adjective = 4,
        Interjection = 5,
        Pronoun = 6,
        Preposition = 7
    }
    public enum CaseTypes
    {
        Nominative = 0,
        Genitive = 1,
        Dative = 2,
        Accusative = 3,
        Ablative = 4,
        Vocative = 5,
        Locative = 6
    }
    public enum GenderTypes
    {
        Masuculine = 0,
        Feminine = 1,
        Neuter = 2
    }

    public class Verbum
    {
        public VerbumTypes VerbumType
        {
            get;
            set;
        }
    }
    public class Noun : Verbum
    {
        public Noun()
        {
            VerbumType = VerbumTypes.Noun;
        }
        public NumberTypes NumberType
        {
            get;
            set;
        }
        public CaseTypes CaseType
        {
            get;
            set;
        }
        public string BaseForm
        {
            get;
            set;
        }
        public override string ToString()
        {
            return String.Format(
@"
VerbumType: Noun
CaseType:{0}
NumberType:{1}
BaseForm:{2}
", CaseType, NumberType, BaseForm);
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
        public CaseTypes CaseType
        {
            get;
            set;
        }
        public GenderTypes GenderType
        {
            get;
            set;
        }
        public NumberTypes NumberType
        {
            get;
            set;
        }
        public string BaseForm
        {
            get;
            set;
        }
        public override string ToString()
        {
            return String.Format(
@"
VerbumType: Participle
CaseType:{0}
GenderType:{1}
NumberType:{2}
BaseForm:{3}
", CaseType, GenderType, NumberType, BaseForm);
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
                                                        switch (liNode.SelectSingleNode("./span/a[1]").InnerText)
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
                                                    else if (liNode.InnerHtml.Contains("supine"))
                                                    {
                                                        ;
                                                    }
                                                    else
                                                    {
                                                        newVerb.VerbType = VerbTypes.Conjugation;

                                                        if (liNode.SelectNodes("./span/a").Count == 5)
                                                        {
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
                                                        else if (liNode.SelectNodes("./span/a").Count == 4)
                                                        {

                                                            switch (liNode.SelectSingleNode("./span/a[1]").InnerText)
                                                            {
                                                                case "singular":
                                                                    newVerb.NumberType = NumberTypes.Singular;
                                                                    break;
                                                                case "plural":
                                                                    newVerb.NumberType = NumberTypes.Plural;
                                                                    break;
                                                            }
                                                            switch (liNode.SelectSingleNode("./span/a[2]").InnerText)
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
                                                            switch (liNode.SelectSingleNode("./span/a[3]").InnerText)
                                                            {
                                                                case "active":
                                                                    newVerb.VoiceType = VoiceTypes.Active;
                                                                    break;
                                                                case "passive":
                                                                    newVerb.VoiceType = VoiceTypes.Passive;
                                                                    break;
                                                            }
                                                            switch (liNode.SelectSingleNode("./span/a[4]").InnerText)
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
                                                        else if(liNode.SelectNodes("./span/a").Count == 6)
                                                        {
                                                            newVerb.TenseType = TenseTypes.FuturePerfect;
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
                                        else if (sectionName == "Participle")
                                        {
                                            HtmlNode pNode = nextNode.NextSibling.NextSibling;
                                            HtmlNode olNode = pNode.NextSibling.NextSibling;

                                            //Console.WriteLine("Verb Found");
                                            if (pNode.ChildNodes.Count > 5)
                                            {
                                                Participle newParticiple = new Participle();
                                                newParticiple.BaseForm = pNode.SelectSingleNode("./strong[@class='Latn headword']").InnerText;
                                                newParticiple.CaseType = CaseTypes.Nominative;
                                                newParticiple.GenderType = GenderTypes.Masuculine;
                                                newParticiple.NumberType = NumberTypes.Singular;
                                                verbumList.Add(newParticiple);
                                            }
                                            else
                                            {
                                                foreach (HtmlNode liNode in olNode.SelectNodes("./li"))
                                                {
                                                    Participle newParticiple = new Participle();
                                                    newParticiple.BaseForm = liNode.SelectSingleNode(".//i/a").InnerText;
                                                    switch (liNode.SelectSingleNode("./span/a[1]").InnerText)
                                                    {
                                                        case "nominative":
                                                            newParticiple.CaseType = CaseTypes.Nominative;
                                                            break;
                                                        case "genitive":
                                                            newParticiple.CaseType = CaseTypes.Genitive;
                                                            break;
                                                        case "dative":
                                                            newParticiple.CaseType = CaseTypes.Dative;
                                                            break;
                                                        case "accusative":
                                                            newParticiple.CaseType = CaseTypes.Accusative;
                                                            break;
                                                        case "ablative":
                                                            newParticiple.CaseType = CaseTypes.Ablative;
                                                            break;
                                                        case "vocative":
                                                            newParticiple.CaseType = CaseTypes.Vocative;
                                                            break;
                                                    }
                                                    switch (liNode.SelectSingleNode("./span/a[2]").InnerText)
                                                    {
                                                        case "masculine":
                                                            newParticiple.GenderType = GenderTypes.Masuculine;
                                                            break;
                                                        case "feminine":
                                                            newParticiple.GenderType = GenderTypes.Feminine;
                                                            break;
                                                        case "neuter":
                                                            newParticiple.GenderType = GenderTypes.Feminine;
                                                            break;

                                                    }
                                                    switch (liNode.SelectSingleNode("./span/a[3]").InnerText)
                                                    {
                                                        case "singular":
                                                            newParticiple.NumberType = NumberTypes.Singular;
                                                            break;
                                                        case "plural":
                                                            newParticiple.NumberType = NumberTypes.Plural;
                                                            break;
                                                    }

                                                    verbumList.Add(newParticiple);
                                                }
                                            }
                                        }
                                        else if (sectionName == "Noun")
                                        {
                                            HtmlNode pNode = nextNode.NextSibling.NextSibling;
                                            HtmlNode olNode = pNode.NextSibling.NextSibling;

                                            //Console.WriteLine("Verb Found");
                                            if (pNode.ChildNodes.Count > 5)
                                            {
                                                Noun newNoun = new Noun();

                                                newNoun.BaseForm = pNode.SelectSingleNode("./strong[@class='Latn headword']").InnerText;


                                                newNoun.CaseType = CaseTypes.Nominative;
                                                newNoun.NumberType = NumberTypes.Singular;
                                                verbumList.Add(newNoun);
                                            }
                                            else
                                            {
                                                foreach (HtmlNode liNode in olNode.SelectNodes("./li"))
                                                {
                                                    Noun newNoun = new Noun();
                                                    if (liNode.SelectSingleNode(".//i/a") != null)
                                                    {
                                                        newNoun.BaseForm = liNode.SelectSingleNode(".//i/a").InnerText;
                                                    }
                                                    else
                                                    {
                                                        newNoun.BaseForm = liNode.SelectSingleNode(".//i/strong").InnerText;
                                                    }
                                                    
                                                    switch (liNode.SelectSingleNode("./span/a[1]").InnerText)
                                                    {
                                                        case "nominative":
                                                            newNoun.CaseType = CaseTypes.Nominative;
                                                            break;
                                                        case "genitive":
                                                            newNoun.CaseType = CaseTypes.Genitive;
                                                            break;
                                                        case "dative":
                                                            newNoun.CaseType = CaseTypes.Dative;
                                                            break;
                                                        case "accusative":
                                                            newNoun.CaseType = CaseTypes.Accusative;
                                                            break;
                                                        case "ablative":
                                                            newNoun.CaseType = CaseTypes.Ablative;
                                                            break;
                                                        case "vocative":
                                                            newNoun.CaseType = CaseTypes.Vocative;
                                                            break;
                                                    }

                                                    switch (liNode.SelectSingleNode("./span/a[2]").InnerText)
                                                    {
                                                        case "singular":
                                                            newNoun.NumberType = NumberTypes.Singular;
                                                            break;
                                                        case "plural":
                                                            newNoun.NumberType = NumberTypes.Plural;
                                                            break;
                                                    }

                                                    verbumList.Add(newNoun);
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
            string queryString = "labor virum vocat";
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
                    foreach (Verbum v in verbumList)
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
