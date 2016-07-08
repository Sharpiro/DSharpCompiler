import {Injectable} from "@angular/core";
import {Http, Response} from "@angular/http";
import {Observable} from "rxjs/Rx";
import {IVehicleService} from "./vehicles";

@Injectable()
export class VehicleService implements IVehicleService
{
    constructor(private httpService: Http) { }

    public getVehicles(): Observable<IBaseData[]>
    {
        var obs = this.httpService.get("/api/vehicles/getdata")
            .map(response => <IBaseData[]>(response.json().data));
        return obs;
    }

    public getVehicle(id: number): Observable<IBaseData>
    {
        var promise = this.getVehicles().map(vehicles => vehicles.find(vehicle => vehicle.id === id));
        return promise;
    }

    public compile(source: string): Observable<string>
    {
        var body = { source: source };
        var obs = this.httpService.post("/api/compiler/compile", body)
            .catch(() =>
            {
                toastr.error("Compilation Error");
                return null;
            })
            .map((response: Response) => JSON.stringify(response.json().data.output));
        return obs;
    }
}