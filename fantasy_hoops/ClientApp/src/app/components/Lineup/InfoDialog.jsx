import React from 'react';
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import Dialog from "@material-ui/core/Dialog";

function InfoDialog(props) {
    const {onDialogOpen, onDialogClose, open} = props;
    
    return (
        <Dialog
            onClose={onDialogClose}
            aria-labelledby="lineup-info-dialog"
            open={open}
        >
            <DialogTitle id="lineup-info-dialog" onClose={onDialogClose}>
                INFO
            </DialogTitle>
            <DialogContent dividers style={{width: '80vw', maxWidth: '40rem'}}>
                <table className="table table-hover">
                    <thead style={{width: '100%'}}>
                    <tr>
                        <th scope="row" colSpan="2" className="InfoModal__thead text-center text-light">Offensive
                        </th>
                    </tr>
                    </thead>
                    <tbody style={{height: 'auto', overflow: 'auto'}}>
                    <tr>
                        <th scope="row" className="text-left">Points</th>
                        <td className="text-right text-green">1 FP</td>
                    </tr>
                    <tr>
                        <th scope="row" className="text-left">Assists</th>
                        <td className="text-right text-green">1.5 FP</td>
                    </tr>
                    <tr>
                        <th scope="row" className="text-left">Turnovers</th>
                        <td className="text-right text-red">-1 FP</td>
                    </tr>
                    </tbody>
                    <thead className="bg-secondary" style={{width: '100%'}}>
                    <tr>
                        <th scope="row" colSpan="2" className="InfoModal__thead text-center text-light">
                            Defensive
                        </th>
                    </tr>
                    </thead>
                    <tbody style={{height: 'auto', overflow: 'auto'}}>
                    <tr>
                        <th scope="row" className="text-left">Rebounds</th>
                        <td className="text-right text-green">1.2 FP</td>
                    </tr>
                    <tr>
                        <th scope="row" className="text-left">Steals</th>
                        <td className="text-right text-green">3 FP</td>
                    </tr>
                    <tr>
                        <th scope="row" className="text-left">Blocks</th>
                        <td className="text-right text-green">3 FP</td>
                    </tr>
                    </tbody>
                </table>
                <hr/>
                * FP - Fantasy Points
            </DialogContent>
            <DialogActions>
                <Button onClick={onDialogClose} color="primary">
                    Close
                </Button>
            </DialogActions>
        </Dialog>
    );
}

export default InfoDialog;