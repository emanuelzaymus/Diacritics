//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiacriticsProject1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class UniGramEntity
    {
        public string Word1 { get; set; }

        [Index]
        public int WordId { get; set; }

        public int Id { get; set; }

        public int Frequency { get; set; }

        public virtual Word Word { get; set; }
    }
}
