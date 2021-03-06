using System;
using System.Text.RegularExpressions;
using MediaBrowser.Model.Extensions;

namespace MediaBrowser.Model.Dlna
{
    public class SearchCriteria
    {
        public SearchType SearchType { get; set; }

        /// <summary>
        /// Splits the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="term">The term.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>System.String[].</returns>
        private static string[] RegexSplit(string str, string term, int limit)
        {
            return new Regex(term).Split(str, limit);
        }

        /// <summary>
        /// Splits the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="term">The term.</param>
        /// <returns>System.String[].</returns>
        private static string[] RegexSplit(string str, string term)
        {
            return Regex.Split(str, term, RegexOptions.IgnoreCase);
        }

        public SearchCriteria(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                throw new ArgumentNullException(nameof(search));
            }

            SearchType = SearchType.Unknown;

            string[] factors = RegexSplit(search, "(and|or)");
            foreach (string factor in factors)
            {
                string[] subFactors = RegexSplit(factor.Trim().Trim('(').Trim(')').Trim(), "\\s", 3);

                if (subFactors.Length == 3)
                {

                    if (StringHelper.EqualsIgnoreCase("upnp:class", subFactors[0]) &&
                        (StringHelper.EqualsIgnoreCase("=", subFactors[1]) || StringHelper.EqualsIgnoreCase("derivedfrom", subFactors[1])))
                    {
                        if (StringHelper.EqualsIgnoreCase("\"object.item.imageItem\"", subFactors[2]) || StringHelper.EqualsIgnoreCase("\"object.item.imageItem.photo\"", subFactors[2]))
                        {
                            SearchType = SearchType.Image;
                        }
                        else if (StringHelper.EqualsIgnoreCase("\"object.item.videoItem\"", subFactors[2]))
                        {
                            SearchType = SearchType.Video;
                        }
                        else if (StringHelper.EqualsIgnoreCase("\"object.container.playlistContainer\"", subFactors[2]))
                        {
                            SearchType = SearchType.Playlist;
                        }
                        else if (StringHelper.EqualsIgnoreCase("\"object.container.album.musicAlbum\"", subFactors[2]))
                        {
                            SearchType = SearchType.MusicAlbum;
                        }
                    }
                }
            }
        }
    }
}
