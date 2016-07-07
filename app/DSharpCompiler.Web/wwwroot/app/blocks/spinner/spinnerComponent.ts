import {Component, OnInit, OnDestroy} from "@angular/core"
import {Subscription} from "rxjs/Rx"
import {SpinnerService} from "./spinnerService"

@Component({
    selector: "spinner",
    template: `<div class="spinner mdl-spinner mdl-js-spinner" [class.is-active]="isVisible"></div>`,
    styles: [`.spinner{position: absolute; left: 46%;top: 35%}`]
})
export class SpinnerComponent implements OnInit, OnDestroy
{
    private isVisible: boolean;
    private _spinnerStateSubscription: Subscription;

    constructor(private _spinnerService: SpinnerService) { }

    public ngOnInit(): void
    {
        this._spinnerStateSubscription = this._spinnerService.spinnerState.subscribe(state => this.isVisible = state.show);
    }

    public ngOnDestroy(): void
    {
        this._spinnerStateSubscription.unsubscribe();
    }
}