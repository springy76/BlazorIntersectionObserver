namespace Blazor.IntersectionObserver.Components
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using API;

    using Microsoft.AspNetCore.Components;

    public class IntersectionObserveBase : ComponentBase, IDisposable
    {
        private static readonly Func<IntersectionObserverEntry, double> entryByTime = e => e.Time;

        [Inject] private IntersectionObserverService ObserverService { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public string Style { get; set; }

        [Parameter] public RenderFragment<IntersectionObserverEntry> ChildContent { get; set; }

        [Parameter] public bool IsIntersecting { get; set; }

        [Parameter] public EventCallback<bool> IsIntersectingChanged { get; set; }

        [Parameter] public EventCallback<IntersectionObserverEntry> OnChange { get; set; }

        [Parameter] public IntersectionObserverOptions Options { get; set; }

        [Parameter] public bool Once { get; set; }

        public ElementReference Element { get; set; }

        public IntersectionObserverEntry Entry { get; set; }

        private IntersectionObserver Observer { get; set; }

        private bool Observing { get; set; }

        protected override void OnAfterRender()
        {
            if ( !this.Observing )
            {
                this.InitialiseObserver();
                this.Observing = true;
            }
        }

        private async void InitialiseObserver()
        {
            this.Observer = await this.ObserverService.Observe(this.Element, this.OnIntersectUpdate, this.Options);
        }

        private void OnIntersectUpdate(IList<IntersectionObserverEntry> entries)
        {
            var entry = entries.OrderByDescending(entryByTime).FirstOrDefault();
            if ( entry == null )
                return;

            async Task ProcessEntryAsync()
            {
                if ( this.Observer == null )
                {
                    Debug.Assert(this.Once && this.Entry?.IsIntersecting == true, "this.Once && this.Entry?.IsIntersecting == true");
                    return;
                }

                await this.IsIntersectingChanged.InvokeAsync(entry.IsIntersecting);
                await this.OnChange.InvokeAsync(entry);
                this.Entry = entry;
                this.StateHasChanged();

                if ( this.Once && entry.IsIntersecting )
                {
                    var observer = this.Observer;
                    this.Observer = null;
                    observer?.Disconnect();
                }
            }

            _ = Task.Run(() => this.InvokeAsync(ProcessEntryAsync));
        }

        public void Dispose()
        {
            this.Observer?.Disconnect();
        }
    }
}
