using UnityEngine;
using UnityEngine.UIElements;

public class TrackpadVisualizer : MonoBehaviour
{
    [SerializeField] TrackpadInput trackpadInput = null;

    UIDocument uiDocument;
    VisualElement root;
    VisualElement trackpadArea;
    VisualElement[] touchIndicators;
    Label infoLabel;

    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            uiDocument = gameObject.AddComponent<UIDocument>();
        }

        CreateUI();
    }

    void CreateUI()
    {
        root = new VisualElement();
        root.style.flexGrow = 1;
        root.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f);

        var container = new VisualElement();
        container.style.flexGrow = 1;
        container.style.alignItems = Align.Center;
        container.style.justifyContent = Justify.Center;

        var title = new Label("Trackpad Touch Visualizer");
        title.style.fontSize = 24;
        title.style.color = Color.white;
        title.style.marginBottom = 20;
        container.Add(title);

        trackpadArea = new VisualElement();
        trackpadArea.style.width = 600;
        trackpadArea.style.height = 400;
        trackpadArea.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
        trackpadArea.style.borderBottomWidth = 2;
        trackpadArea.style.borderTopWidth = 2;
        trackpadArea.style.borderLeftWidth = 2;
        trackpadArea.style.borderRightWidth = 2;
        var borderColor = new Color(0.4f, 0.4f, 0.4f);
        trackpadArea.style.borderBottomColor = borderColor;
        trackpadArea.style.borderTopColor = borderColor;
        trackpadArea.style.borderLeftColor = borderColor;
        trackpadArea.style.borderRightColor = borderColor;
        trackpadArea.style.position = Position.Relative;
        container.Add(trackpadArea);

        touchIndicators = new VisualElement[10];
        for (int i = 0; i < 10; i++)
        {
            var indicator = new VisualElement();
            indicator.style.position = Position.Absolute;
            indicator.style.width = 40;
            indicator.style.height = 40;
            indicator.style.borderBottomLeftRadius = 20;
            indicator.style.borderBottomRightRadius = 20;
            indicator.style.borderTopLeftRadius = 20;
            indicator.style.borderTopRightRadius = 20;
            indicator.style.backgroundColor = GetTouchColor(i);
            indicator.style.display = DisplayStyle.None;

            var label = new Label((i + 1).ToString());
            label.style.color = Color.white;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            label.style.fontSize = 14;
            label.style.flexGrow = 1;
            indicator.Add(label);

            trackpadArea.Add(indicator);
            touchIndicators[i] = indicator;
        }

        infoLabel = new Label("Waiting for touch input...");
        infoLabel.style.color = Color.white;
        infoLabel.style.marginTop = 20;
        infoLabel.style.fontSize = 14;
        container.Add(infoLabel);

        root.Add(container);
        uiDocument.rootVisualElement.Add(root);
    }

    void Update()
    {
        if (trackpadArea == null || touchIndicators == null) return;

        if (trackpadInput == null || !trackpadInput.IsAvailable)
        {
            infoLabel.text = "Trackpad not available";
            return;
        }

        var touches = trackpadInput.CurrentTouches;

        for (int i = 0; i < 10; i++)
        {
            if (i < touches.Length)
            {
                var touch = touches[i];
                touchIndicators[i].style.display = DisplayStyle.Flex;

                var x = touch.normalizedX * trackpadArea.resolvedStyle.width - 20;
                var y = (1f - touch.normalizedY) * trackpadArea.resolvedStyle.height - 20;

                touchIndicators[i].style.left = x;
                touchIndicators[i].style.top = y;

                var size = 20 + touch.force * 30;
                touchIndicators[i].style.width = size;
                touchIndicators[i].style.height = size;
                var radius = size / 2;
                touchIndicators[i].style.borderBottomLeftRadius = radius;
                touchIndicators[i].style.borderBottomRightRadius = radius;
                touchIndicators[i].style.borderTopLeftRadius = radius;
                touchIndicators[i].style.borderTopRightRadius = radius;

                var opacity = 0.5f + touch.force * 0.5f;
                var color = GetTouchColor(i);
                color.a = opacity;
                touchIndicators[i].style.backgroundColor = color;
            }
            else
            {
                touchIndicators[i].style.display = DisplayStyle.None;
            }
        }

        if (touches.Length > 0)
        {
            var primaryTouch = touches[0];
            infoLabel.text = $"Touches: {touches.Length} | " +
                           $"Primary: ({primaryTouch.normalizedX:F3}, {primaryTouch.normalizedY:F3}) | " +
                           $"Force: {primaryTouch.force:F2}";
        }
        else
        {
            infoLabel.text = "No active touches";
        }
    }

    Color GetTouchColor(int index)
    {
        var colors = new Color[]
        {
            new Color(1f, 0.3f, 0.3f),
            new Color(0.3f, 1f, 0.3f),
            new Color(0.3f, 0.3f, 1f),
            new Color(1f, 1f, 0.3f),
            new Color(1f, 0.3f, 1f),
            new Color(0.3f, 1f, 1f),
            new Color(1f, 0.6f, 0.3f),
            new Color(0.6f, 0.3f, 1f),
            new Color(0.3f, 1f, 0.6f),
            new Color(1f, 0.3f, 0.6f)
        };
        return colors[index % colors.Length];
    }
}