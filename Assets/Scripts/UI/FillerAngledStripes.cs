using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class FillerAngledStripes : VisualElement
{
    public const string SPRITE_PATH_PREFIX = "Sprites/";
    public const float STRIPE_SIZE = 32;

    public Color stripeColor { get; set; }
    public float k { get; set; }

    public FillerAngledStripes()
    {
        focusable = false;
        pickingMode = PickingMode.Ignore;
        AddToClassList("filler");
        AddToClassList("filler-angled-stripes");

        style.flexGrow = 1f;
        style.overflow = Overflow.Hidden;

        generateVisualContent += DrawCanvas;
    }


    void DrawCanvas(MeshGenerationContext mgc)
    {
        Painter2D painter2D = mgc.painter2D;

        painter2D.fillColor = stripeColor;
        
        float y1 = contentRect.y;
        float y2 = contentRect.y + contentRect.height;
        for (float x = contentRect.x - y2 * k; x < contentRect.x + contentRect.width; x += STRIPE_SIZE * 2)
        {
            painter2D.BeginPath();
            painter2D.MoveTo(new Vector3(x                       , y1, 0));
            painter2D.LineTo(new Vector3(x + y2 * k              , y2, 0));
            painter2D.LineTo(new Vector3(x + y2 * k + STRIPE_SIZE, y2, 0));
            painter2D.LineTo(new Vector3(x + STRIPE_SIZE         , y1, 0));
            painter2D.ClosePath();
            painter2D.Fill();
        }
    }


    public new class UxmlFactory : UxmlFactory<FillerAngledStripes, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlColorAttributeDescription stripeColorAttribute = new UxmlColorAttributeDescription { name = "stripe-color", defaultValue = Color.white };
        UxmlFloatAttributeDescription kAttribute = new UxmlFloatAttributeDescription { name = "param-k", defaultValue = 1f };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            FillerAngledStripes cve = (FillerAngledStripes) ve;
            cve.stripeColor = stripeColorAttribute.GetValueFromBag(bag, cc);
            cve.k = kAttribute.GetValueFromBag(bag, cc);
        }
    }
}
