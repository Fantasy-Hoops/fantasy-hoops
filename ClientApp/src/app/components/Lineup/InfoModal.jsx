import React from 'react';

export default () => (
  <div className="modal fade" id="infoModal" tabIndex="-1" role="dialog" aria-labelledby="infoModalLabel" aria-hidden="true">
    <div className="modal-dialog" role="document">
      <div className="modal-content">
        <div className="modal-header">
          <h1 className="modal-title" id="infoModalLabel">Scoring Info</h1>
          <a className="close" data-dismiss="modal" aria-label="Close">
            <i style={{ fontSize: '2.5rem' }} width="50rem" className="fas fa-times" />
          </a>
        </div>
        <div className="InfoModal modal-body">
          <table className="table table-hover">
            <thead style={{ width: '100%' }}>
              <tr>
                <th scope="row" colSpan="2" className="InfoModal__thead text-center text-light">Offensive</th>
              </tr>
            </thead>
            <tbody style={{ height: 'auto', overflow: 'auto' }}>
              <tr>
                <th scope="row" className="text-left">Points</th>
                <td className="text-right text-success">1 FP</td>
              </tr>
              <tr>
                <th scope="row" className="text-left">Assists</th>
                <td className="text-right text-success">1.5 FP</td>
              </tr>
              <tr>
                <th scope="row" className="text-left">Turnovers</th>
                <td className="text-right text-danger">-1 FP</td>
              </tr>
            </tbody>
            <thead className="bg-secondary" style={{ width: '100%' }}>
              <tr>
                <th scope="row" colSpan="2" className="InfoModal__thead text-center text-light">
                  Defensive
                </th>
              </tr>
            </thead>
            <tbody style={{ height: 'auto', overflow: 'auto' }}>
              <tr>
                <th scope="row" className="text-left">Rebounds</th>
                <td className="text-right text-success">1.2 FP</td>
              </tr>
              <tr>
                <th scope="row" className="text-left">Steals</th>
                <td className="text-right text-success">3 FP</td>
              </tr>
              <tr>
                <th scope="row" className="text-left">Blocks</th>
                <td className="text-right text-success">3 FP</td>
              </tr>
            </tbody>
          </table>
          <hr />
          * FP - Fantasy Points
        </div>
      </div>
    </div>
  </div>
);
