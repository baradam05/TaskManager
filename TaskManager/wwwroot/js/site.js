function showExpendable(id) {
    let content = document.getElementById(id);
    let iconDown = document.getElementById("iconDown_" + id);
    let iconUp = document.getElementById("iconUp_" + id);

    const isVisible = !content.hasAttribute("hidden");
    if (isVisible) {
        content.setAttribute("hidden", "");
        iconUp.setAttribute("hidden", "");
        iconDown.removeAttribute("hidden");
    } else {
        content.removeAttribute("hidden");        
        iconUp.removeAttribute("hidden");
        iconDown.setAttribute("hidden", "");
    }
}

function expandSubTasks(id) {
    let subTasks = document.getElementById("subTasks_" + id);
    if (subTasks) {
        subTasks.hidden = !subTasks.hidden;
    }
} 

function areAllSubTasksChecked(assignmentId) {
    const checkboxes = document.querySelectorAll(`#subTasks_${assignmentId} input[type="checkbox"][name="completedSubIds"]`);
    if (checkboxes.length === 0) return false;
        return Array.from(checkboxes).every(cb => cb.checked);
}

document.addEventListener("DOMContentLoaded", function () {
    const saveButtons = document.querySelectorAll(".save-btn");

    saveButtons.forEach(function (btn) {
        btn.addEventListener("click", function (e) {
            const assignmentId = btn.getAttribute("data-assignment-id");
            const isFinished = btn.getAttribute("data-finished") === "true"

            const checkboxes = document.querySelectorAll(
                'input[type="checkbox"][data-assignment-id="' + assignmentId + '"]'
            );

            let counter = 0;
            checkboxes.forEach(cb => {
                if (cb.checked) counter++;
            });

            const totalSubTasks = checkboxes.length;
            console.log(`Checked: ${counter}, Total: ${totalSubTasks}`);

            if (totalSubTasks > 0 && counter === totalSubTasks) {
                console.log("All sub-tasks are checked. Prompting for finish confirmation.");
                if (!confirm('All sub-tasks are done. Are sure you want to finish the whole task ?')) {
                    e.preventDefault();
                }
            }
            else if (isFinished && counter !== totalSubTasks) {
                console.log("Task is finished but not all sub-tasks are checked. Prompting for un-finish confirmation.");
                if (!confirm('This task is already finished. Are you sure you want to un-finish it?')) {
                    e.preventDefault();
                }
            } else {
                console.log("No confirmation needed.");
            }
        });
    });
});

