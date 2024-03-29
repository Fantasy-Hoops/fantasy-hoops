import React, {Component} from 'react';
import shortid from 'shortid';
import _ from 'lodash';

export class Error extends Component {
    render() {
        let status = _.map(this.props.status.toString().split(''),
            (letter) => {
                return (
                    <span className="letter" key={shortid()}>
            {letter}
          </span>
                );
            }
        );

        return (
            <div className="letters text-center" style={{marginTop: '10%'}}>
                <img src={require('../../content/images/jordan-crying.png')} style={{height: '200px'}}
                     alt="Sad Jordan"/>
                <br/>
                <span className="letter">E</span>
                <span className="letter">r</span>
                <span className="letter">r</span>
                <span className="letter">o</span>
                <span className="letter">r</span>
                <span className="letter"></span>
                {status}

                <div className="mt-2">
                    <p>
                        {this.props.message}
                    </p>
                </div>
            </div>
        );
    }
}