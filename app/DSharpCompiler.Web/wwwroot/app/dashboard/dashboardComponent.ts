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
export class DashboardComponent implements OnInit, AfterViewInit
{
    public vehicles: Observable<IBaseData[]>;
    public selectedVehicle: IBaseData;
    public transformMe: string;
    @Output() changed = new EventEmitter<IBaseData>();
    private errorMessage: any;
    private isActive = false;

    constructor( @Inject("IVehicleServiceToken") private dataService: IVehicleService, private cdrService: ChangeDetectorRef)
    {
        this.transformMe = "data123";
    }

    public ngOnInit(): void
    {
        componentHandler.upgradeDom();
        this.isActive = false;
        this.vehicles = this.dataService.getVehicles();
    }

    public ngAfterViewInit(): void
    {
        this.isActive = false;
        this.cdrService.detectChanges();
    }

    public select(vehicle: IBaseData): void
    {
        this.selectedVehicle = vehicle;
        this.changed.emit(this.selectedVehicle);
    }
}