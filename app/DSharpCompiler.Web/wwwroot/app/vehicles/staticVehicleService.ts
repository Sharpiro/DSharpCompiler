import {Injectable} from '@angular/core';
import {Http, Response} from '@angular/http';
import {Observable, Operator} from "rxjs/Rx"
import {IVehicleService} from "./vehicles"
import {SpinnerService} from "../blocks/blocks"

@Injectable()
export class StaticVehicleService implements IVehicleService
{
    constructor(private httpService: Http, private _spinnerService: SpinnerService) { }

    public getVehicles(): Observable<IBaseData[]>
    {
        this._spinnerService.show();
        var promise = this.httpService.get("/api/vehicles")
            .do(data => console.log())
            .catch(this.handleError)
            .finally(() => this._spinnerService.hide())
            .map((response: Response) => <IBaseData[]>(response.json().data))

        //var promise = Observable.create((observer: any) =>
        //{
        //    var data: IBaseData[] = [{ id: 1, name: "whatever", type: "space" }, { id: 2, name: "whatever2", type: "space" }];
        //    observer.next(data);
        //    observer.complete();
        //});
        return promise;
    }

    public getVehicle(id: number): Observable<IBaseData>
    {
        var promise = this.getVehicles().map(vehicles => vehicles.find(vehicle => vehicle.id === id));
        return promise;
    }

    private handleError(error: Response): Observable<any>
    {
        console.error(error);
        var obs = Observable.throw("Server Error");
        return obs;
    }
}