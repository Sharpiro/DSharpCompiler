import {Component} from "@angular/core"
import {RouteConfig, ROUTER_DIRECTIVES} from "@angular/router-deprecated"
import {VehicleComponent, VehicleListComponent} from "./vehicles"

@Component({
    selector: "story-vehicles-root",
    template: "<router-outlet></router-outlet>",
    directives: ROUTER_DIRECTIVES
})
@RouteConfig([
    { path: "/", name: "Vehicles", component: VehicleListComponent, useAsDefault: true },
    { path: "/:id", name: "Vehicle", component: VehicleComponent }
])
export class VehiclesComponent { }