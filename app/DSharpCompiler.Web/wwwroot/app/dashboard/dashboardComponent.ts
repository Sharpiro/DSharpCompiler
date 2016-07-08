import {Component, OnInit, Inject, EventEmitter, Output, AfterViewInit, ChangeDetectorRef} from "@angular/core"
import {Observable} from "rxjs/Rx"
import {IVehicleService} from "../vehicles/vehicles"
import {CustomPipe, NestedComponent, LowerCasePipe} from "../blocks/blocks"

@Component({
    selector: "my-app",
    templateUrl: "./app/dashboard/dashboardComponent.html",
    styleUrls: ["./app/dashboard/dashboardComponent.css"],
    directives: [NestedComponent],
    pipes: [CustomPipe, LowerCasePipe],
})
export class DashboardComponent implements OnInit
{
    private defaultInput = 
        `BEGIN
            BEGIN
                number := 2;
                a := number;
                b := 10 * a + 10 * number / 4;
                c := a - - b;
            END;
            x := 11;
            d := 7 + 3 * (10 / (12 / (3 + 1) - 1)) / (2 + 3) - 5 - 3 + (8);
            e := 5 - - - + - (3 + 4) - +2
        END.`
    private input = this.defaultInput;
    private output: Observable<any>;

    constructor( @Inject("IVehicleServiceToken") private dataService: IVehicleService, private cdrService: ChangeDetectorRef)
    {
    }

    public ngOnInit(): void
    {
        //this.output = this.dataService.compile(this.input);
    }

    private compile(): void
    {
        this.output = this.dataService.compile(this.input);
    }

    private reset(): void
    {
        this.input = this.defaultInput;
        this.output = undefined;
    }
}