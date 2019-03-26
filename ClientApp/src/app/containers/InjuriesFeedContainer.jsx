import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import _ from 'lodash';
import shortid from 'shortid';
import InjuryCard from '../components/Injuries/InjuryCard';
import { EmptyJordan } from '../components/EmptyJordan';
import * as actionCreators from '../actions/injuries';

const mapStateToProps = state => ({
  injuries: state.injuriesContainerReducer.injuries,
  injuryLoader: state.injuriesContainerReducer.injuryLoader
});

const mapDispatchToProps = dispatch => bindActionCreators(actionCreators, dispatch);

export class InjuriesFeedContainer extends Component {
  async componentDidMount() {
    const { loadInjuries } = this.props;
    await loadInjuries();

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
    const { injuries, injuryLoader } = this.props;
    if (injuryLoader) {
      return (
        <div className="m-5">
          <div className="Loader" />
        </div>
      );
    }

    if (injuries && injuries.length === 0) {
      return (
        <div className="p-5">
          <EmptyJordan message="No injuries report today..." />
        </div>
      );
    }

    const injuryCards = _.map(injuries, (injury, index) => {
      const animated = index === 0 ? 'animated pulse delay-2s' : '';
      return <InjuryCard key={shortid()} injury={injury} animated={animated} />;
    });
    return (
      <div className="p-0 mt-3 container bg-light">
        <div className="row">{injuryCards}</div>
      </div>
    );
  }
}

InjuriesFeedContainer.propTypes = {
  loadInjuries: PropTypes.func.isRequired,
  injuries: PropTypes.arrayOf(
    PropTypes.shape({
      injuryID: PropTypes.number.isRequired
    })
  ).isRequired,
  injuryLoader: PropTypes.bool.isRequired
};

export default connect(mapStateToProps, mapDispatchToProps)(InjuriesFeedContainer);
