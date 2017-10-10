import { Component, OnInit } from '@angular/core';
import { trigger, transition, style, animate, state } from '@angular/animations'

import { AlertService } from '../../services';

@Component({
    selector: 'alert',
    animations: [
        trigger(
            'myAnimation',
            [
                transition(
                    ':enter', [
                        style({ transform: 'translateY(-100%)', opacity: 0 }),
                        animate('500ms ease-in', style({ transform: 'translateY(0)', 'opacity': 1 }))
                    ]
                ),
                transition(
                    ':leave', [
                        style({ transform: 'translateY(0)', 'opacity': 1 }),
                        animate('500ms ease-out', style({ transform: 'translateY(-100%)', 'opacity': 0 }))
                    ]
                )]
        )
    ],
    templateUrl: 'alert.component.html',
    styleUrls: ['./alert.component.css']
})

export class AlertComponent {
    message: any;

    constructor(private alertService: AlertService) { }

    ngOnInit() {
        this.alertService.getMessage().subscribe(message => {
            this.message = message;
            setTimeout(() => {
                this.message = null;
            }, 3500);
        });
    }
}