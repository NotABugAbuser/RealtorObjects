using RealtyModel.Model.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model
{
    static class Pagination
    {
        public static string[] Paginate(int pagesTotal, int currentPage) {
            List<string> pages = new List<string>();
            if (pagesTotal == 1) {
                pages.Add("1");
                return pages.ToArray();
            }
            if (currentPage > 1) {
                pages.Add("<");
            }
            if (currentPage != 1) {
                pages.Add("1");
            }
            if (currentPage - 5 > 1) {
                pages.Add("...");
            }
            for (int i = 5; i >= 1; i--) {
                if (currentPage - i > 1) {
                    pages.Add($"{currentPage - i}");
                }
            }
            pages.Add(currentPage.ToString());
            if (currentPage == pagesTotal) {
                return pages.ToArray();
            } else {
                for (int i = 1; i <= 5; i++) {
                    if (currentPage + i <= pagesTotal - 1) {
                        pages.Add($"{currentPage + i}");
                    }
                }
                if (currentPage + 5 < pagesTotal - 1) {
                    pages.Add("...");
                }
                pages.Add(pagesTotal.ToString());
                if(currentPage < pagesTotal) {
                    pages.Add(">");
                }
                return pages.ToArray();
            }
        }
        public static List<ObservableCollection<BaseRealtorObject>> Split(List<BaseRealtorObject> filteredList, byte pageSize = 25) {
            List<ObservableCollection<BaseRealtorObject>> pageList = new List<ObservableCollection<BaseRealtorObject>>();
            if (filteredList.Count > pageSize) {
                foreach (IEnumerable<BaseRealtorObject> batch in filteredList.Batch(pageSize)) {
                    pageList.Add(new ObservableCollection<BaseRealtorObject>(batch));
                }
            } else {
                pageList.Add(new ObservableCollection<BaseRealtorObject>(filteredList));
            }
            return pageList;
        }
    }
}
