import {Observable} from "rxjs/Rx";
import {OpaqueToken} from "@angular/core";

export interface ICompilerService
{
    compilePascal(source: string): Observable<any>;
    compileDSharp(source: string): Observable<any>;
    compileCSharp(source: string): Observable<any>;
    transpileCSharp(source: string): Observable<any>;
    getSyntaxTree(source: string): Observable<any>;
}