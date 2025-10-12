using System;
using System.IO;

namespace TIMS_X.Core.Extensions
{
    public static class UriBuilderExtensions
    {
        public static UriBuilder AppendToPath(this UriBuilder builder, string pathToAdd)
        {
            var completePath = Path.Combine(builder.Path, pathToAdd);
            builder.Path = completePath;
            return builder;
        }
    }
}
