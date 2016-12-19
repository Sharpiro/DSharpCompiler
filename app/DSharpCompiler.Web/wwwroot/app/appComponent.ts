import {Component, provide} from "@angular/core"
import {Location} from "@angular/common"
import {RouteConfig, ROUTER_DIRECTIVES } from "@angular/router-deprecated"
import {HTTP_PROVIDERS} from "@angular/http"
import {CsharpComponent, DashboardComponent, CompilerService, SpinnerComponent, SpinnerService, AboutComponent} from "./appCore"

@Component({
    selector: "my-app",
    directives: [ROUTER_DIRECTIVES, DashboardComponent, SpinnerComponent],
    providers: [HTTP_PROVIDERS, provide("ICompilerServiceToken", { useClass: CompilerService }),
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
    { path: "/csharp", name: "CSharp", component: CsharpComponent },
    { path: "/about", name: "About", component: AboutComponent }
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