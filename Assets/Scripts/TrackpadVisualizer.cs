using UnityEngine;
using UnityEngine.UIElements;
using TouchPoint = TrackpadPlugin.TouchPoint;
using System.Linq;

public class TrackpadVisualizer : MonoBehaviour
{
    [SerializeField] TrackpadInput trackpadInput = null;

    Label[] _indicators;

    Color GetIDColor(int id)
      => Color.HSVToRGB(id * 0.3f % 1, 0.6f, 0.9f);

    void HidaIndicator(Label indicator)
      => indicator.style.display = DisplayStyle.None;

    void UpdateIndicator(Label indicator, in TouchPoint touch)
    {
        indicator.style.display = DisplayStyle.Flex;
        indicator.style.backgroundColor = GetIDColor(touch.touchID);
        indicator.style.left = Length.Percent(touch.normalizedX * 100);
        indicator.style.bottom = Length.Percent(touch.normalizedY * 100);
        indicator.text = $"{touch.touchID}";
        indicator.transform.scale = Vector3.one * touch.force;
    }

    void Start()
    {
        _indicators = Enumerable.Range(0, 32).Select(i => new Label()).ToArray();

        var area = GetComponent<UIDocument>().rootVisualElement.Q("trackpad-area");
        foreach (var e in _indicators)
        {
            e.AddToClassList("indicator");
            area.Add(e);
        }

#if !UNITY_EDITOR
        UnityEngine.Cursor.visible = false;
#endif
    }

    void Update()
    {
        var touches = trackpadInput.CurrentTouches;

        for (var i = 0; i < touches.Length; i++)
            UpdateIndicator(_indicators[i], touches[i]);

        for (var i = touches.Length; i < _indicators.Length; i++)
            HidaIndicator(_indicators[i]);
    }
}