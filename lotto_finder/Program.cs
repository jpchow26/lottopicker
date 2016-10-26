using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace lotto_finder
{
    class Program
    {
        static void Main(string[] args)
        {
            //string[] testset = { "1", "42", "100848", "4938532894754", "1234567", "472844278465445" };

            if (args.Length != 1)
            {
                Console.WriteLine("Please supply text file to scan for lotto numbers.");
                Console.WriteLine("Press any key to quit.");
                Console.ReadKey();
            }


            string filepath = args[0] as string;
            string text;
            if (File.Exists(filepath)) // load text and regex all number strings 7-14 digits long
            {
                text = File.ReadAllText(filepath);
                var matches = getNumbers(text);

                Console.WriteLine(matches.Count + " possible lotto numbers in file.");
                int count = 0;
                foreach (var match in matches)
                {
                    string numString = match.ToString();

                    if (parseDigits(numString, numString.Length - 7, 0, 0, new Dictionary<int, int>(), 0))
                        count++;
                }
                Console.WriteLine(count + " lotto numbers found.");
            }

            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
        }

        static MatchCollection getNumbers(string input)
        {
            return Regex.Matches(input, @"(\d{7,14})");
        }

        static bool isValidNumber(int val, Dictionary<int,int> dic)
        {
            if (val < 60 && val > 0 && !dic.ContainsValue(val))
                return true;
            else
                return false;
        }

        static bool isStringPossibleLottoString(string val)
        {
            var num = Convert.ToInt64(val);
            if (num >= 1234567 && num <= 59585756555453) // smallest and largest possible string match
                return true;
            else
                return false;
        }


        static void printDictionary(Dictionary<int, int> dic)
        {
            string original = string.Join("", dic.Values);
            string parsed = string.Join(" ", dic.Values);
            Console.WriteLine(original + " => " + parsed);
        }

        static bool parseDigits(string input, int k, int parsedPairs, int parsedNumbers, Dictionary<int, int> result, int charIndex)
        {
            int singleAdds = 0;
            if (parsedPairs == k) // found all the pairs, so single up the rest of the string
            {
                for (int i = charIndex; i < input.Length; i++)
                {
                    var digit = Convert.ToInt32(input.Substring(i, 1));

                    if (!isValidNumber(digit,result))
                    {
                        break;
                    }
                    result.Add(parsedNumbers++, digit);
                    singleAdds++;
                }
            }

            // if all pairs found AND the right number of lotto numbers success
            if (parsedNumbers == (input.Length - (k*2) + k ) && parsedPairs == k)
            { 
                printDictionary(result); // print out result 

                // loop for finding other combinations in the same string
                //for (int i = 0; i < singleAdds; i++ )
                //{
                //    result.Remove(--parsedNumbers);
                //} 
                // for now just return true on first valid combination
                return true;
            }
            else
            {   // wipe out the single digits, total # or wrong number of pairs
                for (int i = 0; i < singleAdds; i++)
                {
                    result.Remove(--parsedNumbers);
                }
            }
            
            // iterate the string
            for (int i = charIndex; i < input.Length-1; )
            {
                var twoDigit = Convert.ToInt16(input.Substring(i, 2));
                if (isValidNumber(twoDigit, result))
                {
                    result.Add(parsedNumbers++, twoDigit); // add two digits at start index 
                    parsedPairs++;
                    if (parseDigits(input, k, parsedPairs, parsedNumbers, result, i + 2)) // recurse on next two digit number
                        return true; //quit the loop if the recursive call found a match
                    result.Remove(--parsedNumbers); //otherwise remove last added number on result
                    parsedPairs--;
                }
                var singleDigit = Convert.ToInt32(input.Substring(i++, 1)); // add single digit at start index
                if (!isValidNumber(singleDigit, result))
                    break; // both single and double digits at charIndex failed, just quit the loop bad string
                result.Add(parsedNumbers++, singleDigit);
            }

            // this recursive call was a bust, remove all the last added single digit numbers
            foreach (var element in result.OrderByDescending(x => x.Key))
            {
                if (element.Value < 10)
                    result.Remove(element.Key);
                else
                    break;
            }
            return false;
        }

    }
}
