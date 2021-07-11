using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model
{
    static class Pagination
    {
        public static void Paginate(int current, int last) {
            int delta = 2;
            int left = current - delta;
            int right = current + delta + 1;
            List<int> range = new List<int>();
            List<string> rangeWithDots = new List<string>();
            int l = 0;
            for (int i = 1; i <= last; i++) {
                if (i == 1 || i == last || i>= left && i < right) {
                    range.Add(i);
                }
            }
            foreach (int i in range) {
                if (l != 0) {
                    if (i - 1 == 2) {
                        rangeWithDots.Add((l + 1).ToString());
                    } else if(i - 1 != 1) {
                        rangeWithDots.Add("...");
                    }
                }
                rangeWithDots.Add(i.ToString());
                l = i;
            }
            foreach(string str in rangeWithDots) {
                Debug.Write(str);
            }
        }
        /*
    for (let i of range) {
        if (l) {
            if (i - l === 2) {
                rangeWithDots.push(l + 1);
            } else if (i - l !== 1) {
                rangeWithDots.push('...');
            }
        }
        rangeWithDots.push(i);
        l = i;
    }

    return rangeWithDots;
}*/
    }
}
