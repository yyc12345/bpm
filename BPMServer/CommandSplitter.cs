using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMServer {
    public static class CommandSplitter {

        public static List<string> SplitCommand(string command) {

            var result = new List<string>();
            //==========================check 
            if (command == "" || command == string.Empty) return result;
            int quotation = 0;
            foreach (var item in command) {
                if (item == '"') quotation++;
            }
            if (quotation % 2 != 0) return result;

            //==========================split
            //make sure splitter can work. add a space
            command += " ";

            int quotationCount = 0;
            string cacheItem = string.Empty;
            foreach (var item in command) {
                if (item == '"') quotationCount++; //ignore quotation
                else if (item == ' ') {
                    //push data
                    if (quotationCount % 2 == 0) {
                        //judge the main command
                        result.Add(cacheItem);
                        //clear string
                        cacheItem = string.Empty;
                    } else cacheItem += item; //save space
                } else cacheItem += item; //save word
            }

            return result;
        }

    }
}
