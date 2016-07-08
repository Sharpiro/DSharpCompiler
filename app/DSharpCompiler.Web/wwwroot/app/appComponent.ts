import {Component, provide, OpaqueToken} from "@angular/core"
import {Location} from "@angular/common"
import {RouteConfig, ROUTER_DIRECTIVES, ROUTER_PROVIDERS } from "@angular/router-deprecated"
import {InMemoryBackendService, SEED_DATA}  from "angular2-in-memory-web-api/";
import {InMemoryDb} from "../fakeApi/inMemoryDb"
import {HTTP_PROVIDERS, XHRBackend} from "@angular/http"
import {DashboardComponent, StaticVehicleService, VehicleService, VehiclesComponent,
    VehicleListComponent, VehicleComponent, SpinnerComponent, SpinnerService, AboutComponent} from "./appCore"

@Component({
    selector: "my-app",
    directives: [ROUTER_DIRECTIVES, DashboardComponent, SpinnerComponent],
    providers: [HTTP_PROVIDERS, provide("IVehicleServiceToken", { useClass: VehicleService }),
        //provide(XHRBackend, { useClass: InMemoryBackendService }),
        //provide(SEED_DATA, { useClass: InMemoryDb }),
        SpinnerService],
    templateUrl: "./app/appComponent.html",
    styles: [
        `nav ul {list-style-type: none;}
        nav ul li {padding: 4px;display:inline-block}
        bg-color { background-color: #ECECEA }
`
    ]
})
@RouteConfig([
    { path: "/dashboard", name: "Dashboard", component: DashboardComponent, useAsDefault: true },
    { path: "/about", name: "About", component: AboutComponent },
    { path: "/vehicles/...", name: "Vehicles", component: VehiclesComponent }
])
export class AppComponent
{
    public showNavValue = "";


    constructor(private _location: Location)
    {

    }

    public changed(changedCharacter: IBaseData): void
    {
        const message = `Event changed: ${changedCharacter.name}`;
        toastr.success(message);
        console.log(message);
    }

    private isActive(path: string): string
    {
        const currentPath = this._location.path();
        const result = currentPath === path ? "active" : null;
        return result;
    }

    private toggleNav(): void
    {
        if (this.showNavValue === "in")
            this.showNavValue = "";
        else
            this.showNavValue = "in";
    }
}