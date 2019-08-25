using Microsoft.AspNetCore.Components;

namespace Blazor.IntersectionObserver.API
{
    public class IntersectionObserverEntry
    {
        /* DOM-Element can't be serialized to Server-Side-Blazor: public ElementReference Target { get; set; }*/

        public bool IsIntersecting { get; set; }

        public double IntersectionRatio { get; set; }

        public DOMRectReadOnly BoundingClientRect { get; set; }

        public DOMRectReadOnly RootBounds { get; set; }

        public bool IsVisible { get; set; }

        public double Time { get; set; }
    }
}
