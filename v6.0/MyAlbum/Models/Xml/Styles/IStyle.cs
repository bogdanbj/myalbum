using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlbum.Models.Xml.Styles
{
    /// <summary>
    /// Interface for all style types to support generic style finding.
    /// </summary>
    public interface IStyle
    {
        public string Style { get; }
        public bool IsDefault { get; }
    }
}
