import React, { PureComponent } from 'react';
import Scroll from 'react-scroll';
import shortid from 'shortid';
import _ from 'lodash';
import Img from 'react-image';
import defaultLogo from '../../../content/images/defaultLogo.png';

export class NewsCard extends PureComponent {
  constructor(props) {
    super(props);
    this.state = {
      checked: false
    };
    this.handleCheck = this.handleCheck.bind(this);
  }

  handleCheck(e) {
    if (this.state.checked) {
      const div = e.target.parentElement.parentElement;
      const position = div.getBoundingClientRect().top - 80;
      const duration = (position * -1) / 15;
      if (position < 0) {
        Scroll.animateScroll.scrollMore(position, {
          duration,
          smooth: true
        });
      }
    }
    this.setState({
      checked: !this.state.checked
    });
  }

  render() {
    const size = 3;
    const paragraphs = _.map(this.props.news.paragraphs,
      paragraph => <p key={shortid()}>{paragraph}</p>);
    return (
      <div className="NewsCard mb-5 mx-auto news-card card">
        <div className="card-header bg-primary text-white">
          <h3 className="card-title">
            {this.props.news.title}
          </h3>
        </div>
        <span>
          <div className="position-absolute">
            <Img
              className="NewsCard__TeamLogo"
              alt={this.props.news.hTeam}
              src={[
                `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/logos/${this.props.news.vTeam}.svg`,
                defaultLogo
              ]}
              loader={<img height="50px" src={require('../../../content/images/imageLoader.gif')} alt="Loader" />}
              decode={false}
            />
          </div>
        </span>
        <span style={{ paddingLeft: '7rem' }}>
          <div className="position-absolute">
            <Img
              className="NewsCard__TeamLogo"
              alt={this.props.news.vTeam}
              src={[
                `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/logos/${this.props.news.hTeam}.svg`,
                defaultLogo
              ]}
              loader={<img height="50px" src={require('../../../content/images/imageLoader.gif')} alt="Loader" />}
              decode={false}
            />
          </div>
        </span>
        <div className="NewsCard__Info card-header text-muted">


          vs
          <span style={{ float: 'right' }}>
            {this.props.news.date}
          </span>
        </div>
        <div className="card-body">
          <input
            onChange={this.handleCheck}
            checked={this.state.checked}
            type="checkbox"
            className="read-more-state"
            id={this.props.news.id}
          />
          <div className="read-more-wrap" style={{ textAlign: 'justify' }}>
            {paragraphs.slice(0, size)}

            {!this.state.checked ? '' : ''}
            <span className="read-more-target">
              {!this.state.checked ? '' : paragraphs.slice(size)}
            </span>

          </div>
          <label htmlFor={this.props.news.id} className="read-more-trigger" />
        </div>
      </div>
    );
  }
}

export default NewsCard;
