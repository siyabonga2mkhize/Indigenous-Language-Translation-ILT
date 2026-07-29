using System.ComponentModel.DataAnnotations;

namespace PhraseBookk.Models
{
    public enum ContentStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public enum LanguageCode
    {
        [Display(Name = "English")]
        en = 0,

        [Display(Name = "Afrikaans")]
        af = 1,

        [Display(Name = "isiZulu")]
        zu = 2,

        [Display(Name = "isiXhosa")]
        xh = 3,

        [Display(Name = "Sesotho")]
        st = 4,

        [Display(Name = "Sepedi")]
        nso = 5,

        [Display(Name = "Setswana")]
        tn = 6,

        [Display(Name = "Xitsonga")]
        ts = 7,

        [Display(Name = "siSwati")]
        ss = 8,

        [Display(Name = "Tshivenda")]
        ve = 9,

        [Display(Name = "isiNdebele")]
        nr = 10
    }
}


