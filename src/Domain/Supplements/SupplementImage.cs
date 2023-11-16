using Ardalis.GuardClauses;
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Supplements;

public class SupplementImage : Entity
{
    public Supplement? Supplement { get; }
    public Uri? Image { get; }

    public SupplementImage(Uri image, Supplement supplement)
    {
        Guard.Against.Null(supplement, nameof(Supplement));
        Guard.Against.Null(image, nameof(Image));
        Supplement = supplement;
        Image = image;
    }

    private SupplementImage()
    {

    }
}
