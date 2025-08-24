using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace VisuALS_WPF_App
{
    public class TextData
    {
        public int caretIndex;
        public string text;

        public TextData(int caretindex, string str)
        {
            caretIndex = caretindex;
            text = str;
        }
    }
    public class Predictor
    {
        private SqlDatabase predictorData = new SqlDatabase(); //the SQL database that holds the predictor data
        public delegate string StringProcessingFunc(string str); //A delegate for processing a string
        public delegate List<List<string>> WordsProcessingFunc(List<string> words); //A delegate for proccessing a list of words
        public string commonWordsPath = "";
        public List<string> commonWords = new List<string>(); //holds a list of common words loaded from a text file
        private readonly char[] _tokenDelimeter = { ' ' }; // Delimeter between words
        Stopwatch stopwatch = new Stopwatch(); //used for diagnostics (testing prediction speeds)
        List<long> times = new List<long>(); //used for diagnostics
        public enum PredictionType { NEXT_WORD, WORD_COMPLETE }; //passed into the ApplyPrediction function when called to
                                                                 //determine how to properly insert the prediction into the text
        public Predictor() { }

        public void SetDatabasePath(string filepath)
        {
            predictorData.Open(filepath);
        }

        public void SetupDatabase()
        {
            //setup the prediction database, will not create a new DB if one already exists
            SqlColumnDefinitions columns = new SqlColumnDefinitions();
            columns.Add("id", "bigint");
            columns.Add("word1", "string");
            columns.Add("word2", "string");
            columns.Add("word3", "string");
            columns.Add("prediction", "string");
            columns.Add("frequency", "double");
            predictorData.AddTable("next_word_data", columns);
        }

        public void SetCommonWordsList(string filepath)
        {
            commonWordsPath = filepath;
        }

        public void LoadCommonWords()
        {
            commonWords = File.ReadAllLines(commonWordsPath).ToList();
        }

        /// <summary>
        /// Starts a persistent connection to the predictor database to decrease
        /// overhead of accessing the data. This connection MUST be manually closed
        /// with the ClosePersistentConnection method when you are done using the
        /// reduced overhead connection.
        /// </summary>
        public void OpenPersistentConnection()
        {
            predictorData.StartPersistentConnection();
        }

        /// <summary>
        /// Ends a persistent connection to the predictor database after being
        /// started with the OpenPersistentConnection method.
        /// </summary>
        public void ClosePersistentConnection()
        {
            predictorData.EndPersistentConnection();
        }

        /// <summary>
        /// Get predictions about what word someone may be trying to type, this should be used
        /// when the caret is inside or right at the end of a word
        /// </summary>
        /// <param name="input"> the complete text box contents </param>
        /// <param name="caretIndex"> the index of the caret in the text box </param>
        /// <param name="numOfEntries"> number of predictions to retrieve </param>
        /// <param name="onlyPerfectCompletions"> only get completions which exactly match the letters preceding the caret </param>
        /// <returns></returns>
        public List<string> FinishWordPredict(string input, int caretIndex, int numOfEntries = 3, bool onlyPerfectCompletions = false)
        {
            //get the partial word directly preceding the caret
            string lastWord = RemoveAllPunctuationExceptgApostrophe(input);
            lastWord = GetTextPrecedingCaret(input, caretIndex).Split(' ').ToList().Last();

            //get all other text preceding the caret
            string allButLastWord = input.Remove(input.LastIndexOf(lastWord));

            //this will store the word completion predictions
            List<string> completeWords = new List<string>();

            //get contextual possible matches
            List<string> nextWords = NextWordPredict(allButLastWord, caretIndex, 50);

            if (onlyPerfectCompletions) //if using only perfect completions
            {
                //get data both from the common word list and from the context aware next word prediction data
                //but only entries which are perfect matches for the letters preceding the caret
                completeWords.AddRange(nextWords.Where(x => isValidCompletion(lastWord, x)).ToList());
                completeWords.AddRange(commonWords.Where(x => isValidCompletion(lastWord, x)).ToList());
            }
            else
            {
                //get context matches
                completeWords.AddRange(PartialStringMatching.GetNearMatches(lastWord, nextWords));
                //add perfect word matches from the common words list as well
                completeWords.AddRange(PartialStringMatching.GetNearMatches(lastWord, commonWords));
            }

            //remove extra predictions if there are more than requested
            if (completeWords.Count > numOfEntries)
            {
                completeWords = completeWords.GetRange(0, numOfEntries);
            }

            return completeWords;
        }

        /// <summary>
        /// An asyncronous version of FinishWordPredict
        /// </summary>
        /// <param name="input"> the complete text box contents </param>
        /// <param name="caretIndex"> the index of the caret in the text box </param>
        /// <param name="numOfEntries"> number of predictions to retrieve </param>
        /// <param name="onlyPerfectCompletions"> only get completions which exactly match the letters preceding the caret </param>
        /// <param name="useNearbyKeyReplacements"> if allowing partial matches, this will allow the partial matcher to attempt replacing 
        ///                                         letters with other letters that are nearby on the keyboard </param>
        /// <returns></returns>
        public async Task<List<string>> FinishWordPredictAsync(string input, int caretIndex, int numOfEntries = 3, bool onlyPerfectCompletions = false)
        {
            Func<List<string>> finishWord = () => App.predictor.FinishWordPredict(input, caretIndex, numOfEntries, onlyPerfectCompletions); // Get Predictions
            return await Task.Run(finishWord);
        }

        /// <summary>
        /// checks to see if a completion begins with a target string
        /// in other words it checks if the completion is a valid perfect match
        /// prediction for the target string
        /// </summary>
        /// <param name="target"> target string, what you want the prediction for </param>
        /// <param name="completion"> the possible word completion prediction </param>
        /// <returns></returns>
        private bool isValidCompletion(string target, string completion)
        {
            int targetLength = target.Length;
            if (completion.Contains(target))
            {
                if (completion.Substring(0, targetLength) == target.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Applies a prediction to a text box. It does not actually take a text box
        /// and manipulate it, instead it takes the data and returns what the new
        /// data ought to be, the caller is tasked with applying this new data to the
        /// text box, this makes the code more versatile, as it does not actually
        /// require a textbox element to use.
        /// </summary>
        /// <param name="text"> the text from the text box </param>
        /// <param name="prediction"> the prediction to apply </param>
        /// <param name="caretIndex"> the caret index of the text box </param>
        /// <param name="predictType"> the type of prediction (word completion or next word) </param>
        /// <returns></returns>
        public TextData ApplyPrediction(string text, string prediction, int caretIndex, PredictionType predictType)
        {
            TextData newTextData = new TextData(caretIndex, text); //this is where we will store the new text box data to return

            //capture the current caret index, and what the new index should be
            int caret = newTextData.caretIndex;

            if (predictType == PredictionType.NEXT_WORD || (predictType == PredictionType.WORD_COMPLETE && GetTextFollowingCaret(newTextData.text, newTextData.caretIndex) == ""))
            {
                prediction = prediction + " ";
            }
            if (predictType == PredictionType.WORD_COMPLETE)
            {
                //get some important data about what text is before and after the caret
                int numberOfCharsPrecedingCaret = GetTextPrecedingCaret(newTextData.text, newTextData.caretIndex).Length;
                int i = GetTextFollowingCaret(newTextData.text, newTextData.caretIndex).IndexOf(' ');
                int j = GetTextPrecedingCaret(newTextData.text, newTextData.caretIndex).LastIndexOf(' ');

                int indexOfEndOfCurrentWord;
                int indexOfBeginningOfCurrentWord;
                if (i == -1)
                {
                    indexOfEndOfCurrentWord = newTextData.text.Length;
                }
                else
                {
                    indexOfEndOfCurrentWord = numberOfCharsPrecedingCaret + i;
                }

                if (j == -1)
                {
                    indexOfBeginningOfCurrentWord = 0;
                }
                else
                {
                    indexOfBeginningOfCurrentWord = j + 1;
                }

                int sizeOfCurrentWord = indexOfEndOfCurrentWord - indexOfBeginningOfCurrentWord;

                //use all that data to figure out what text to remove when applying a word completion prediction
                newTextData.text = newTextData.text.Remove(indexOfBeginningOfCurrentWord, sizeOfCurrentWord);

                caret = indexOfBeginningOfCurrentWord;
            }

            //insert the prediction into the string and put that in the new text data
            newTextData.text = newTextData.text.Insert(caret, prediction);

            //put the new caret into the new text data
            newTextData.caretIndex = caret + prediction.Length;

            return newTextData;
        }

        /// <summary>
        /// Gets the text preceding a caret index
        /// </summary>
        /// <param name="str"> string to get text from </param>
        /// <param name="caretIndex"> index of caret </param>
        /// <returns></returns>
        private string GetTextPrecedingCaret(string str, int caretIndex)
        {
            if (caretIndex == 0)
            {
                return "";
            }
            else
            {
                if (caretIndex > str.Length)
                {
                    return str;
                }
                else
                {
                    return str.Substring(0, caretIndex);
                }
            }
        }

        /// <summary>
        /// Get the text after a caret index in a string
        /// </summary>
        /// <param name="str"> string to get text from </param>
        /// <param name="caretIndex"> caret index </param>
        /// <returns></returns>
        private string GetTextFollowingCaret(string str, int caretIndex)
        {
            string s = str.Substring(caretIndex, str.Length - caretIndex);
            return s;
        }

        /// <summary>
        /// Train the predictor with the input and return the next word predictions
        /// this prediction function uses the predictor SQL database and is context
        /// aware up to the last 3 words
        /// </summary>
        /// <param name="input"> complete text box contents </param>
        /// <param name="caretIndex"> index of caret in the text box </param>
        /// <param name="numberOfResults"> number of predictions to get </param>
        /// <returns></returns>
        public List<string> NextWordPredict(string input, int caretIndex, int numberOfResults = 3)
        {
            //get the text before the caret, as that is the only text used for the prediction
            input = GetTextPrecedingCaret(input, caretIndex);

            //clean the input from extraneous characters
            List<string> words = CleanEntries(input.Split(' ').ToList()).First();
            //remove words that were "destroyed" during cleaning (empty strings)
            words.RemoveAll(x => x == "");

            //the target will be a list of the 3 (or less) words preceding the
            //caret
            string[] target;

            //this will hold the predictions fetched from the database
            List<string> result;

            //get the target words from the input, if there are fewer than 3 target words, use
            //empty strings to keep the target count at 3
            if (words.Count > 2)
            {
                target = new string[] { words[words.Count - 3], words[words.Count - 2], words.Last() };
            }
            else if (words.Count == 2)
            {
                target = new string[] { "", words[0], words[1] };
            }
            else if (words.Count == 1)
            {
                target = new string[] { "", "", words[0] };
            }
            else
            {
                target = new string[] { "", "", "" };
            }

            //attempt to find a perfect match for the target in the database
            //if the number of predictions is less than 3 it will fall through to the next
            //level and try searching for only the last two targets, and then if that fails
            //to get 3 prediction it will try matching only the last target
            if (predictorData.Count("next_word_data", "word1 = \"" + target[0] + "\" and word2 = \"" + target[1] + "\" and word3 = \"" + target[2] + "\"") > 2)
            {
                result = predictorData.GetMultipleVals<string>("next_word_data", "prediction", "word1 = \"" + target[0] + "\" and word2 = \"" + target[1] + "\" and word3 = \"" + target[2] + "\"", true, SqlDatabase.Sort.DESC, "frequency", numberOfResults);
            }
            else if (predictorData.Count("next_word_data", "word2 = \"" + target[1] + "\" and word3 = \"" + target[2] + "\"") > 2)
            {
                result = predictorData.GetMultipleVals<string>("next_word_data", "prediction", "word2 = \"" + target[1] + "\" and word3 = \"" + target[2] + "\"", true, SqlDatabase.Sort.DESC, "frequency", numberOfResults);
            }
            else if (predictorData.Count("next_word_data", "word3 = \"" + target[2] + "\"") > 0)
            {
                result = predictorData.GetMultipleVals<string>("next_word_data", "prediction", "word3 = \"" + target[2] + "\"", true, SqlDatabase.Sort.DESC, "frequency", numberOfResults);
            }
            else
            {
                result = new List<string>(); //if there were no predictions, return an empty list
            }

            //return the predictions
            return result;
        }

        /// <summary>
        /// An asyncronous version of NextWordPredict
        /// </summary>
        /// <param name="input"> complete text box contents </param>
        /// <param name="caretIndex"> index of caret in the text box </param>
        /// <param name="numberOfResults"> number of predictions to get </param>
        /// <returns></returns>
        public async Task<List<string>> NextWordPredictAsync(string input, int caretIndex, int numberOfResults = 3)
        {
            Func<List<string>> nextWord = () => App.predictor.NextWordPredict(input, caretIndex, numberOfResults); // Get Predictions
            return await Task.Run(nextWord);
        }

        public async void PartialStringTrainAsync(string text, int caretIndex)
        {
            //start training the predictor with the new data in the background
            Action train = () => PartialStringTrain(text, caretIndex, CleanEntries);
            await Task.Run(train);
        }

        /// <summary>
        /// Trains the predictor on an entire string. This involves going through
        /// each word and associating it with the prior 3 words, and then incrementing
        /// the counter for this prediction/context combo if it already exists, or creating
        /// a new entry if it does not
        /// </summary>
        /// <param name="input"> string to train on </param>
        /// <param name="ProcessWords"> a function to proccess the string after it is parsed into words </param>
        public void WholeStringTrain(string input, WordsProcessingFunc ProcessWords = null)
        {
            //parse the words
            List<string> inputWords = input.Split(' ').ToList();
            List<List<string>> processedWords;

            //process the words if a processing function is provided
            //the processing function returns multiple lists so that
            //if words are removed, you don't have two words next to
            //eachother that shouldn't be
            if (ProcessWords == null && inputWords.Count > 0)
            {
                processedWords = new List<List<string>>();
                processedWords.Add(inputWords);
            }
            else
            {
                processedWords = ProcessWords(inputWords);
            }

            //go through each proccessed word list
            foreach (List<string> words in processedWords)
            {
                if (words.Count > 0 && words[0] != "")
                {
                    for (int i = 0; i < words.Count; i++)
                    {
                        string[] target;

                        if (i == 0)
                        {
                            target = new string[] { "", "", "" };
                        }
                        else if (i == 1)
                        {
                            target = new string[] { "", "", words[0] };
                        }
                        else if (i == 2)
                        {
                            target = new string[] { "", words[0], words[1] };
                        }
                        else
                        {
                            target = new string[] { words[i - 3], words[i - 2], words[i - 1] };
                        }

                        //uncomment the following line to print the targets and prediction to debug as it trains
                        //Debug.WriteLine(target[0] == "" ? "-" : target[0] + "\t" + target[1] == "" ? "-" : target[1] + "\t" + target[2] == "" ? "-" : target[2] + "\t" + words[i]);

                        RawQuadTrain(target[0], target[1], target[2], words.Last());
                    }
                }
            }
        }

        /// <summary>
        /// Trains using only the last complete word preceding the caret
        /// This is used to train the set on the users typing, as presumably
        /// they have already typed (and therefore trained the set on) all the
        /// words before the last word, so using this function prevents duplicate
        /// training on user data
        /// </summary>
        /// <param name="input"> the complete text box contents </param>
        /// <param name="caretIndex"> index of caret in text box </param>
        /// <param name="ProcessWords"> function for processing the words prior to training </param>
        public void PartialStringTrain(string input, int caretIndex, WordsProcessingFunc ProcessWords = null)
        {
            //get only the text preceding the caret
            input = GetTextPrecedingCaret(input, caretIndex);

            //parse the string into words
            List<string> words = input.Split(' ').ToList();

            //process the words if a processing function was provided
            if (words.Count > 0 && words[0] != "")
            {
                if (ProcessWords != null)
                {
                    words = ProcessWords(words).Last();
                }
            }

            //get the the target words (the 3 words preceding the word to train on)
            //if there are fewer than 3 words replace missing spots with empty strings
            string[] target;
            if (words.Count > 0 && words[0] != "")
            {
                if (words.Count == 1)
                {
                    target = new string[] { "", "", "" };
                }
                else if (words.Count == 2)
                {
                    target = new string[] { "", "", words[0] };
                }
                else if (words.Count == 3)
                {
                    target = new string[] { "", words[0], words[1] };
                }
                else
                {
                    target = new string[] { words[words.Count - 4], words[words.Count - 3], words[words.Count - 2] };
                }

                RawQuadTrain(target[0], target[1], target[2], words.Last());
            }
        }

        public void RawQuadTrain(string target1, string target2, string target3, string prediction)
        {
            //check if an entry for this context/prediction pair already exists in the database
            if (predictorData.Exists("next_word_data", "prediction", "word1 = \"" + target1 + "\" and word2 = \"" + target2 + "\" and word3 = \"" + target3 + "\" and prediction = \"" + prediction + "\""))
            {
                //if it already exists then fetch the current frequency value
                double freq = Convert.ToDouble(predictorData.GetValue("next_word_data", "frequency", "word1 = \"" + target1 + "\" and word2 = \"" + target2 + "\" and word3 = \"" + target3 + "\" and prediction = \"" + prediction + "\""));

                //increase that frequency value
                SqlRecord newProb = new SqlRecord();
                if (freq < 0.0001)
                {
                    newProb.Add("frequency", freq + 0.1);
                }
                else
                {
                    newProb.Add("frequency", freq + (freq / (Math.Pow(freq, 2) + 0.01)) - 0.1);
                }

                //and update the record
                predictorData.UpdateRecord("next_word_data", newProb, "word1 = \"" + target1 + "\" and word2 = \"" + target2 + "\" and word3 = \"" + target3 + "\" and prediction = \"" + prediction + "\"");

                //find all other records with the same target words
                List<long> recs = predictorData.GetMultipleVals<long>("next_word_data", "id", "word1 = \"" + target1 + "\" and word2 = \"" + target2 + "\" and word3 = \"" + target3 + "\"");

                //and decrease all their frequencies slightly
                foreach (long i in recs)
                {
                    //if it already exists then fetch the current frequency value
                    double f = Convert.ToDouble(predictorData.GetValue("next_word_data", "frequency", "id = \"" + i.ToString() + "\""));

                    //decrease that frequency value
                    SqlRecord p = new SqlRecord();
                    p.Add("frequency", f - (f / (Math.Pow(f, 2) + 0.01)));


                    predictorData.UpdateRecord("next_word_data", p, "id = \"" + i.ToString() + "\"");
                }
            }
            else //if it doesn't exist then create a new entry
            {
                long index; //will store the id for the new entry
                long count = predictorData.Count("next_word_data", "word1 = \"" + target1 + "\" and word2 = \"" + target2 + "\" and word3 = \"" + target3 + "\"");
                if (count > 49)
                {
                    //if there are 50 or more, get the index of the one with the smallest count
                    index = predictorData.GetMultipleVals<long>("next_word_data", "id", "word1 = \"" + target1 + "\" and word2 = \"" + target2 + "\" and word3 = \"" + target3 + "\"", false, SqlDatabase.Sort.ASC, "frequency", 1)[0];
                    //and remove it
                    predictorData.RemoveRecord("next_word_data", "id = \"" + index.ToString() + "\"");
                }
                else
                {
                    //otherwise set the index to the current total count of records in the next_word_data table
                    index = predictorData.Count("next_word_data", "");
                }

                //add the record
                SqlRecord newEntry = new SqlRecord();
                newEntry.Add("id", index);
                newEntry.Add("word1", target1);
                newEntry.Add("word2", target2);
                newEntry.Add("word3", target3);
                newEntry.Add("prediction", prediction);
                newEntry.Add("frequency", 0.1);
                predictorData.AddRecord("next_word_data", newEntry);
            }
        }

        /// <summary>
        /// Train the predictor on a large text file as input
        /// </summary>
        /// <param name="fileLocation"> path of the text file </param>
        /// <param name="lineProcessing"> optional string processing function to apply to each line as the predictor is trained </param>
        public void BookTrain(string fileLocation, StringProcessingFunc lineProcessing = null)
        {
            string line;
            int counter = 0;
            StreamReader file = new StreamReader(fileLocation);

            predictorData.StartPersistentConnection(); //starts a persistent connection to the DB to improved speed

            //go through each line of the file and perform a whole string train on it

            //NOTE: Although the line will not be processed if a function is not provided
            //the WholeStringTrain is passed a CleanEntriesAndSplitAtNonEnglish function for word
            //processing, the line processing function is for preprocessing data that may not
            //be formatted correctly for training
            while ((line = file.ReadLine()) != null)
            {
                if (lineProcessing == null) //if there is not line processing function provided
                {
                    //train without processing the line
                    WholeStringTrain(line, CleanEntriesAndSplitAtNonWord);
                }
                else
                {
                    //if one is provided then apply it before passing to whole string train
                    WholeStringTrain(lineProcessing(line), CleanEntriesAndSplitAtNonWord);
                }
                counter++;
            }

            predictorData.EndPersistentConnection(); //ends the DB connection
        }

        /// <summary>
        /// Train the predictor on a web document
        /// </summary>
        /// <param name="fileURL"> URL of the document </param>
        /// <param name="lineProcessing"> optional string processing function to apply to each line as the predictor is trained </param>
        public void WebTrain(string fileURL, StringProcessingFunc lineProcessing = null)
        {
            string line;
            WebRequest request = WebRequest.Create(@fileURL);

            predictorData.StartPersistentConnection(); //starts a persistent connection to the DB to improve speed


            using (WebResponse response = request.GetResponse())
            {
                using (Stream content = response.GetResponseStream())
                {

                    //go through each line of the file and perform a whole string train on it

                    //NOTE: Although the line will not be processed if a function is not provided
                    //the WholeStringTrain is passed a CleanEntriesAndSplitAtNonEnglish function for word
                    //processing, the line processing function is for preprocessing data that may not
                    //be formatted correctly for training

                    StreamReader file = new StreamReader(content);
                    while ((line = file.ReadLine()) != null)
                    {
                        if (lineProcessing == null) //if there is not line processing function provided
                        {
                            //train without processing the line
                            WholeStringTrain(line, CleanEntriesAndSplitAtNonWord);
                        }
                        else
                        {
                            //if one is provided then apply it before passing to whole string train
                            WholeStringTrain(lineProcessing(line), CleanEntriesAndSplitAtNonWord);
                        }
                    }
                }
            }

            predictorData.EndPersistentConnection(); //ends the DB connection
        }

        /// <summary>
        /// Trains the text predictor on SWIG's public IRC chat logs on the web
        /// The records contain logs for every day from 2004-11-24 through 2018-09-06
        /// </summary>
        /// <param name="startDate"> date of log to start training on </param>
        /// <param name="endDate"> date of last log to train on </param>
        public void TrainUsingIRCLogRecords(DateTime startDate, DateTime endDate)
        {
            //setup variables for tracking the date of the current log to fetch
            int[] daysPerMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            int year = startDate.Year;
            int month = startDate.Month;
            int day = startDate.Day;

            //compute how many records are being requested
            int numberOfRecords = Convert.ToInt32(endDate.Subtract(startDate).TotalDays);

            //if it does not already exist, create a file to store diagnostics data on how
            //long it took to process each log and if the processing was interupted by an
            //exception
            if (!File.Exists("IRCTrainingDataDiagnostics.txt"))
            {
                using (StreamWriter sw = File.CreateText("IRCTrainingDataDiagnostics.txt"))
                {
                    sw.WriteLine("Log Date\tProccessing Time(Min)");
                }
            }

            //go through every log from the start date to the end date provided
            for (int i = 0; i < numberOfRecords; i++)
            {
                try //try to do a web train on the document
                {
                    stopwatch.Restart(); //to get how long it takes to process the file
                    //process the file
                    WebTrain("http://chatlogs.planetrdf.com/swig/" + year.ToString() + "-" + (month < 10 ? "0" : "") + month.ToString() + "-" + (day < 10 ? "0" : "") + day.ToString() + ".txt", ProcessIRCLogTrainingString);
                    stopwatch.Stop(); //stop the timer

                    //log how long it took to process in the diagnostics file
                    using (StreamWriter sw = File.AppendText("IRCTrainingDataDiagnostics.txt"))
                    {
                        sw.WriteLine(year.ToString() + "/" + month.ToString() + "/" + day.ToString() + "\t" + (stopwatch.ElapsedMilliseconds / 60000).ToString());
                    }
                }
                catch (Exception ex) //if it fails due to an exception, log this in the diagnostics file
                {
                    using (StreamWriter sw = File.AppendText("IRCTrainingDataDiagnostics.txt"))
                    {
                        sw.WriteLine(year.ToString() + "/" + month.ToString() + "/" + day.ToString() + "\t" + ex.Message);
                    }
                }

                //once a file has finished processing, write to the debug console which file it was
                Debug.WriteLine("Log Date: " + year.ToString() + "/" + month.ToString() + "/" + day.ToString() + "\tProccessing Time (Min): " + (stopwatch.ElapsedMilliseconds / 60000).ToString());

                //increment the date
                if (day < daysPerMonth[month - 1])
                {
                    day++;
                }
                else
                {
                    if (month < 12)
                    {
                        month++;
                    }
                    else
                    {
                        month = 1;
                        year++;
                    }
                    day = 1;
                }

            }
        }

        /// <summary>
        /// A function to remove extraneous formatting or automated messages from a line of text
        /// from the SWIG IRC public chat log files
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ProcessIRCLogTrainingString(string input)
        {
            if (input.Contains("has quit") || input.Contains("has joined")) //if it was an automated message, return an empty string
            {
                return "";
            }
            else
            {
                int ind = input.IndexOf('>'); //remove time stamp and username
                input = RemoveAllPunctuationExceptgApostrophe(input); //remove extraneous punctuation
                return input;
            }
        }

        /// <summary>
        /// Clean training data of erroneous special characters
        /// Word processing functions return multiple lists so that
        /// if words are removed, you don't have two words next to
        /// eachother that shouldn't be
        /// </summary>
        /// <param name="stringList"></param>
        /// <returns></returns>
        public List<List<string>> CleanEntries(List<string> stringList)
        {
            List<string> clean = new List<string>();
            List<List<string>> cleanList = new List<List<string>>();

            //go through every word in the list
            foreach (string word in stringList)
            {
                //remove extraneous characters
                string cleanWord = RemoveAllPunctuationExceptgApostrophe(word);

                //if it was all just spaces and/or punctuation, ignore it
                if (cleanWord != "")
                {
                    clean.Add(cleanWord); //otherwise add the word to the list
                }
            }
            cleanList.Add(clean);
            return cleanList;
        }

        /// <summary>
        /// Clean training data of erroneous special characters and words not in the common words list
        /// Word processing functions return multiple lists so that
        /// if words are removed, you don't have two words next to
        /// eachother that shouldn't be
        /// </summary>
        /// <param name="stringList"></param>
        /// <returns></returns>
        public List<List<string>> CleanEntriesAndSplitAtNonWord(List<string> stringList)
        {
            List<List<string>> cleanList = new List<List<string>>();
            List<string> clean = new List<string>();

            //go through each word in the list
            foreach (string word in stringList)
            {
                //remove the extraneous characters
                string cleanWord = RemoveAllPunctuationExceptgApostrophe(word);

                if (cleanWord != "") //if it was all just spaces and/or punctuation, ignore it
                {
                    if (commonWords.Contains(cleanWord.ToLower().Replace("'", ""))) //if it was a common word, add it to the list
                    {
                        clean.Add(cleanWord);
                    }
                    else //if it was a non-common word, add the current list to the list of lists, and start a new list
                    {
                        if (clean.Count > 0)
                        {
                            cleanList.Add(clean);
                        }
                        clean = new List<string>();
                    }
                }
            }
            cleanList.Add(clean);

            return cleanList;
        }

        /// <summary>
        /// Very self explanatory, removes all punctuation and special character EXCEPT for apostrophes
        /// </summary>
        /// <param name="str"> string to remove characters from </param>
        /// <returns></returns>
        public string RemoveAllPunctuationExceptgApostrophe(string str)
        {
            string cleanWord = str;
            cleanWord = cleanWord.Replace("\"", "");
            cleanWord = cleanWord.Replace(")", "");
            cleanWord = cleanWord.Replace("(", "");
            cleanWord = cleanWord.Replace(":", "");
            cleanWord = cleanWord.Replace(";", "");
            cleanWord = cleanWord.Replace("\\", "");
            cleanWord = cleanWord.Replace("/", "");
            cleanWord = cleanWord.Replace("-", "");
            cleanWord = cleanWord.Replace("~", "");
            cleanWord = cleanWord.Replace("*", "");
            cleanWord = cleanWord.Replace("=", "");
            cleanWord = cleanWord.Replace("+", "");
            cleanWord = cleanWord.Replace(".", "");
            cleanWord = cleanWord.Replace("?", "");
            cleanWord = cleanWord.Replace("!", "");
            cleanWord = cleanWord.Replace(",", "");
            return cleanWord;
        }
    }
}