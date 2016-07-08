import {bootstrap}    from "@angular/platform-browser-dynamic";
import {provide} from "@angular/core"
import {AppComponent} from "./appComponent";
import {ROUTER_PROVIDERS} from "@angular/router-deprecated"
import {LocationStrategy, HashLocationStrategy, APP_BASE_HREF, PathLocationStrategy} from "@angular/common"
import "rxjs/Rx"

bootstrap(AppComponent, [ROUTER_PROVIDERS, provide(APP_BASE_HREF, { useValue: '/' }),
    provide(LocationStrategy, { useClass: HashLocationStrategy })]);