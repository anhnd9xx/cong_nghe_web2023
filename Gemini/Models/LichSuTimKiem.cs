//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gemini.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class LichSuTimKiem
    {
        public int Id { get; set; }
        public string TuKhoa { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
    
        public virtual TaiKhoanCaNhan TaiKhoanCaNhan { get; set; }
    }
}
