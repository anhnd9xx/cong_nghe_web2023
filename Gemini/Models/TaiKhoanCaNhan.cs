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
    
    public partial class TaiKhoanCaNhan
    {
        public TaiKhoanCaNhan()
        {
            this.BanBes = new HashSet<BanBe>();
            this.LichSuTimKiems = new HashSet<LichSuTimKiem>();
        }
    
        public int Id { get; set; }
        public string Ten { get; set; }
        public string MatKhau { get; set; }
        public string SoDienThoai { get; set; }
        public string LinkAvatar { get; set; }
        public string Token { get; set; }
        public string ChanIds { get; set; }
        public int Active { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    
        public virtual ICollection<BanBe> BanBes { get; set; }
        public virtual ICollection<LichSuTimKiem> LichSuTimKiems { get; set; }
    }
}