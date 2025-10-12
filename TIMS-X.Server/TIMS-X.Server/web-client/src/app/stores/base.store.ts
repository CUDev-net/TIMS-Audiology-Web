
import { observable } from 'mobx';

export class BaseStore {
    @observable public inprogress: boolean = false;
}