import {Component, Output, EventEmitter} from "@angular/core"

@Component({
    selector: "filterText",
    templateUrl: "./app/blocks/filterText/filterTextComponent.html"
})
export class FilterTextComponent
{
    @Output() changed = new EventEmitter<string>();
    private filter: string;

    public clear(): void
    {
        this.filter = "";
    }

    public filterComponentChanged(event: Event): void
    {
        event.preventDefault();
        this.changed.emit(this.filter);
    }
}