import React, { Component } from 'react';
import _ from 'lodash';
import shortid from 'shortid';
import InjuryCard from './InjuryCard';
import { Loader } from '../Loader';
import { EmptyJordan } from '../EmptyJordan';
import { getInjuries } from '../../utils/networkFunctions';

export class InjuriesFeed extends Component {
  _isMounted = false;

  constructor(props) {
    super(props);

    this.state = {
      injuries: [],
      injuryLoader: true
    };
  }

  async componentDidMount() {
    this._isMounted = true;
    await getInjuries()
      .then((res) => {
        if (this._isMounted) {
          this.setState({
            injuries: res.data,
            injuryLoader: false
          });
        }
      });
    const cards = document.querySelectorAll('.InjuryCard');

    function transition() {
      if (this.classList.contains('active')) {
        this.lastChild.classList.add('overflow-hidden');
        this.lastChild.classList.remove('overflow-auto');
        this.classList.remove('active');
      } else if (this.lastChild.lastChild.lastChild.innerHTML !== '') {
        this.lastChild.classList.add('overflow-auto');
        this.lastChild.classList.remove('overflow-hidden');
        this.classList.add('active');
      }
    }
    cards.forEach(card => card.addEventListener('click', transition));
  }

  render() {
    const { injuries, injuryLoader } = this.state;
    if (injuryLoader) {
      return (
        <div className="m-5">
          <Loader show={injuryLoader} />
        </div>
      );
    }

    if (injuries.length === 0) {
      return (
        <div className="p-5">
          <EmptyJordan message="No injuries report today..." />
        </div>
      );
    }

    const injuryCards = _.map(injuries, (injury, index) => {
      const animated = index === 0 ? 'animated pulse delay-2s' : '';
      return (
        <InjuryCard
          key={shortid()}
          injury={injury}
          animated={animated}
        />
      );
    });
    return (
      <div className="p-0 mt-3 container bg-light">
        <div className="row">{injuryCards}</div>
      </div>
    );
  }
}

export default InjuriesFeed;
