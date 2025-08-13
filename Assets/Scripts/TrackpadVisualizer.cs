using UnityEngine;
using UnityEngine.UIElements;

public class TrackpadVisualizer : MonoBehaviour
{
    [SerializeField] TrackpadInput trackpadInput = null;

    VisualElement _area;
    Label[] _indicators;

    void Start()
      => CreateUI();

    void CreateUI()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _area = root.Q("trackpad-area");
        _indicators = new Label[10];

        for (int i = 0; i < _indicators.Length; i++)
        {
            var e = new Label();
            e.AddToClassList("indicator");
            e.style.display = DisplayStyle.None;
            e.style.backgroundColor = Color.HSVToRGB(i * 0.3f, 0.6f, 0.9f);
            e.text = $"{i + 1}";
            _area.Add(e);
            _indicators[i] = e;
        }
    }

    void Update()
    {
        var touches = trackpadInput.CurrentTouches;

        for (int i = 0; i < _indicators.Length; i++)
        {
            var indicator = _indicators[i];

            if (i >= touches.Length)
            {
                indicator.style.display = DisplayStyle.None;
                continue;
            }

            var touch = touches[i];

            indicator.style.display = DisplayStyle.Flex;
            indicator.style.left = Length.Percent(touch.normalizedX * 100);
            indicator.style.bottom = Length.Percent(touch.normalizedY * 100);
            indicator.transform.scale = Vector3.one * touch.force;
        }
    }
}