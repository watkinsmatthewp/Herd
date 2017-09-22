export class User {
    id: number;
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    oAuthToken: string;

    constructor(email: string, password: string, firstName: string, lastName: string) {
        this.email = email;
        this.password = password;
        this.firstName = firstName;
        this.lastName = lastName;
    }
}