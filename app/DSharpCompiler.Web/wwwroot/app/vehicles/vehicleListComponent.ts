import {Component, OnInit, AfterViewInit, Inject, EventEmitter, Output, ViewChild, ChangeDetectorRef} from "@angular/core"
import {ROUTER_DIRECTIVES, RouteConfig} from "@angular/router-deprecated"
import {Observable} from "rxjs/rx"
import {IVehicleService, VehicleComponent } from "./vehicles"
import {NestedComponent, CustomPipe, FilterTextService, FilterTextComponent, LowerCasePipe} from "../blocks/blocks"

@Component({
    selector: "vehicleList",
    templateUrl: "./app/vehicles/vehicleListComponent.html",
    styles: [`.columns {
        -moz-columns: 3;
        -webkit-columns: 3;
        columns: 3;
        }
        .dashboard-button {
            width: 200px;
            height: 70px;
        }`
    ],
    directives: [FilterTextComponent, ROUTER_DIRECTIVES, NestedComponent],
    providers: [FilterTextService],
    pipes: [LowerCasePipe]
})
export class VehicleListComponent implements OnInit, AfterViewInit
{
    public vehicles: IBaseData[];
    public filteredVehicles: IBaseData[];
    public selectedVehicle: IBaseData;
    @Output() changed = new EventEmitter<IBaseData>();
    @ViewChild(FilterTextComponent) filterComponent: FilterTextComponent;

    constructor( @Inject("IVehicleServiceToken") private dataService: IVehicleService,
        private filterService: FilterTextService, private cdrService: ChangeDetectorRef) { }

    public ngOnInit(): void { }

    public ngAfterViewInit(): void
    {
        this.getVehicles();
        this.cdrService.detectChanges();
    }

    public select(vehicle: IBaseData): void
    {
        var temp = this.filterComponent;
        this.selectedVehicle = vehicle;
        this.changed.emit(this.selectedVehicle);
    }

    public getVehicles()
    {
        this.dataService.getVehicles().subscribe(vehicles =>
        {
            this.filteredVehicles = this.vehicles = vehicles
            if (this.filterComponent)
                this.filterComponent.clear();
        });
    }

    public filterListComponentChanged(searchText: string): void
    {
        this.filteredVehicles = this.filterService.filter(searchText, ['id', 'name', 'type'], this.vehicles);
    }
}