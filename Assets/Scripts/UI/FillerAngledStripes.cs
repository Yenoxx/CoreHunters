using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class FillerAngledStripes : VisualElement
{
    public const string SPRITE_PATH_PREFIX = "Sprites/";
    public const float STRIPE_SIZE = 32;


    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        Painter2D painter2D = mgc.painter2D;

        painter2D.fillColor = style.backgroundColor.value;

        for (float x = worldBound.x; x < worldBound.x + worldBound.width; x += STRIPE_SIZE * 2)
        {
            Debug.Log("" + worldBound.x + "; " + worldBound.y + "; " + worldBound.width + "; " + worldBound.height);
            painter2D.BeginPath();
            painter2D.MoveTo(new Vector3(x                  , worldBound.y                    , 0));
            painter2D.LineTo(new Vector3(x + STRIPE_SIZE    , worldBound.y + worldBound.height, 0));
            painter2D.LineTo(new Vector3(x + STRIPE_SIZE * 2, worldBound.y + worldBound.height, 0));
            painter2D.LineTo(new Vector3(x + STRIPE_SIZE    , worldBound.y                    , 0));
            painter2D.ClosePath();
            painter2D.Fill();
        }
    }


    public new class UxmlFactory : UxmlFactory<FillerAngledStripes, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            FillerAngledStripes ate = ve as FillerAngledStripes;

            ate.focusable = false;
            ate.pickingMode = PickingMode.Ignore;
            ate.AddToClassList("filler-element");
        }
    }
}
