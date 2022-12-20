using System.ComponentModel.DataAnnotations;

namespace Demo.IDP.Entities.ViewModels
{
    /// <summary>
    /// Using two step verification
    /// </summary>
    public class TwoStepModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string TwoFactorCode { get; set; }
        public bool RememberLogin { get; set; }
    }
}
